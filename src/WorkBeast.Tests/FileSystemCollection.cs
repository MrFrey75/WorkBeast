using Xunit;

namespace WorkBeast.Tests;

/// <summary>
/// Collection definition to ensure tests that use the file system run sequentially
/// </summary>
[CollectionDefinition("FileSystem", DisableParallelization = true)]
public class FileSystemCollection
{
}
