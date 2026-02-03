using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkBeast.Core.DTOs;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;
using Xunit;

namespace WorkBeast.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager();
        _roleManagerMock = MockRoleManager();
        _configurationMock = new Mock<IConfiguration>();

        // Setup configuration
        _configurationMock.Setup(c => c["Jwt:SecretKey"]).Returns("WorkBeast_Test_Secret_Key_2026_At_Least_32_Characters!");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("WorkBeastTest");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("WorkBeastTestUsers");

        _service = new AuthService(_userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_SuccessfullyCreatesUser()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.NotEmpty(result.Token);
        Assert.Contains("User", result.Roles);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNullWhenUserExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Email = request.Email });

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_SuccessfullyAuthenticatesUser()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var user = new ApplicationUser
        {
            Id = 1,
            Email = request.Email,
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);
        
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNullForInvalidPassword()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new ApplicationUser
        {
            Email = request.Email,
            IsActive = true
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNullForInactiveUser()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var user = new ApplicationUser
        {
            Email = request.Email,
            IsActive = false
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUserWithRoles()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        _userManagerMock.Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User", "Trainer" });

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal(2, result.Roles.Count);
        Assert.Contains("User", result.Roles);
        Assert.Contains("Trainer", result.Roles);
    }

    [Fact]
    public async Task AssignRoleAsync_SuccessfullyAssignsRole()
    {
        // Arrange
        var user = new ApplicationUser { Id = 1 };
        
        _userManagerMock.Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);
        
        _roleManagerMock.Setup(x => x.RoleExistsAsync("Trainer"))
            .ReturnsAsync(true);
        
        _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Trainer"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.AssignRoleAsync(1, "Trainer");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeactivateUserAsync_SuccessfullyDeactivatesUser()
    {
        // Arrange
        var user = new ApplicationUser { Id = 1, IsActive = true };
        
        _userManagerMock.Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);
        
        _userManagerMock.Setup(x => x.UpdateAsync(It.Is<ApplicationUser>(u => !u.IsActive)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.DeactivateUserAsync(1);

        // Assert
        Assert.True(result);
        Assert.False(user.IsActive);
    }

    // Helper methods to mock UserManager and RoleManager
    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var optionsAccessor = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        optionsAccessor.Setup(x => x.Value).Returns(new IdentityOptions());
        
        var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();
        
        var mock = new Mock<UserManager<ApplicationUser>>(
            store.Object, 
            optionsAccessor.Object, 
            passwordHasher.Object, 
            userValidators, 
            passwordValidators, 
            keyNormalizer.Object, 
            errors.Object, 
            services.Object, 
            logger.Object);
        return mock;
    }

    private Mock<RoleManager<ApplicationRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<ApplicationRole>>();
        var roleValidators = new List<IRoleValidator<ApplicationRole>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var logger = new Mock<ILogger<RoleManager<ApplicationRole>>>();
        
        var mock = new Mock<RoleManager<ApplicationRole>>(
            store.Object, 
            roleValidators, 
            keyNormalizer.Object, 
            errors.Object, 
            logger.Object);
        return mock;
    }
}
