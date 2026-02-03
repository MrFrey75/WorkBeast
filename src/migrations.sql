CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "BodyParts" (
    "Oid" INTEGER NOT NULL CONSTRAINT "PK_BodyParts" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsSystem" INTEGER NOT NULL
);

CREATE TABLE "Exercises" (
    "Oid" INTEGER NOT NULL CONSTRAINT "PK_Exercises" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsSystem" INTEGER NOT NULL
);

CREATE TABLE "WorkoutSessions" (
    "Oid" INTEGER NOT NULL CONSTRAINT "PK_WorkoutSessions" PRIMARY KEY AUTOINCREMENT,
    "Date" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsSystem" INTEGER NOT NULL
);

CREATE TABLE "BodyPartExercise" (
    "ExercisesOid" INTEGER NOT NULL,
    "TargetedBodyPartsOid" INTEGER NOT NULL,
    CONSTRAINT "PK_BodyPartExercise" PRIMARY KEY ("ExercisesOid", "TargetedBodyPartsOid"),
    CONSTRAINT "FK_BodyPartExercise_BodyParts_TargetedBodyPartsOid" FOREIGN KEY ("TargetedBodyPartsOid") REFERENCES "BodyParts" ("Oid") ON DELETE CASCADE,
    CONSTRAINT "FK_BodyPartExercise_Exercises_ExercisesOid" FOREIGN KEY ("ExercisesOid") REFERENCES "Exercises" ("Oid") ON DELETE CASCADE
);

CREATE TABLE "ExerciseBodyPart" (
    "ExerciseOid" INTEGER NOT NULL,
    "BodyPartOid" INTEGER NOT NULL,
    CONSTRAINT "PK_ExerciseBodyPart" PRIMARY KEY ("ExerciseOid", "BodyPartOid"),
    CONSTRAINT "FK_ExerciseBodyPart_BodyParts_BodyPartOid" FOREIGN KEY ("BodyPartOid") REFERENCES "BodyParts" ("Oid") ON DELETE CASCADE,
    CONSTRAINT "FK_ExerciseBodyPart_Exercises_ExerciseOid" FOREIGN KEY ("ExerciseOid") REFERENCES "Exercises" ("Oid") ON DELETE CASCADE
);

CREATE TABLE "LoggedExercises" (
    "Oid" INTEGER NOT NULL CONSTRAINT "PK_LoggedExercises" PRIMARY KEY AUTOINCREMENT,
    "WorkoutSessionOid" INTEGER NOT NULL,
    "ExerciseOid" INTEGER NOT NULL,
    "Sequence" INTEGER NOT NULL,
    "ParentGroupId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsSystem" INTEGER NOT NULL,
    CONSTRAINT "FK_LoggedExercises_Exercises_ExerciseOid" FOREIGN KEY ("ExerciseOid") REFERENCES "Exercises" ("Oid") ON DELETE RESTRICT,
    CONSTRAINT "FK_LoggedExercises_WorkoutSessions_WorkoutSessionOid" FOREIGN KEY ("WorkoutSessionOid") REFERENCES "WorkoutSessions" ("Oid") ON DELETE CASCADE
);

CREATE TABLE "WorkoutSets" (
    "Oid" INTEGER NOT NULL CONSTRAINT "PK_WorkoutSets" PRIMARY KEY AUTOINCREMENT,
    "LoggedExerciseOid" INTEGER NOT NULL,
    "Weight" INTEGER NOT NULL,
    "Reps" INTEGER NOT NULL,
    "ResistanceLevel" INTEGER NULL,
    "SetType" TEXT NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsSystem" INTEGER NOT NULL,
    CONSTRAINT "FK_WorkoutSets_LoggedExercises_LoggedExerciseOid" FOREIGN KEY ("LoggedExerciseOid") REFERENCES "LoggedExercises" ("Oid") ON DELETE CASCADE
);

CREATE INDEX "IX_BodyPartExercise_TargetedBodyPartsOid" ON "BodyPartExercise" ("TargetedBodyPartsOid");

CREATE INDEX "IX_ExerciseBodyPart_BodyPartOid" ON "ExerciseBodyPart" ("BodyPartOid");

CREATE INDEX "IX_Exercises_Name" ON "Exercises" ("Name");

CREATE INDEX "IX_LoggedExercises_ExerciseOid" ON "LoggedExercises" ("ExerciseOid");

CREATE INDEX "IX_LoggedExercises_Sequence" ON "LoggedExercises" ("Sequence");

CREATE INDEX "IX_LoggedExercises_WorkoutSessionOid" ON "LoggedExercises" ("WorkoutSessionOid");

CREATE INDEX "IX_WorkoutSessions_Date" ON "WorkoutSessions" ("Date");

CREATE INDEX "IX_WorkoutSets_LoggedExerciseOid" ON "WorkoutSets" ("LoggedExerciseOid");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203150143_InitialCreate', '8.0.5');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY AUTOINCREMENT,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY AUTOINCREMENT,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "LastLoginAt" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" INTEGER NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" INTEGER NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203150730_AddIdentity', '8.0.5');

COMMIT;

BEGIN TRANSACTION;

DROP INDEX "IX_WorkoutSets_LoggedExerciseOid";

DROP INDEX "IX_LoggedExercises_WorkoutSessionOid";

CREATE INDEX "IX_WorkoutSets_LoggedExerciseOid_SortOrder" ON "WorkoutSets" ("LoggedExerciseOid", "SortOrder");

CREATE INDEX "IX_WorkoutSessions_IsDeleted" ON "WorkoutSessions" ("IsDeleted");

CREATE INDEX "IX_LoggedExercises_WorkoutSessionOid_Sequence" ON "LoggedExercises" ("WorkoutSessionOid", "Sequence");

CREATE INDEX "IX_Exercises_IsDeleted" ON "Exercises" ("IsDeleted");

CREATE INDEX "IX_BodyParts_Name" ON "BodyParts" ("Name");

CREATE INDEX "IX_AspNetUsers_CreatedAt" ON "AspNetUsers" ("CreatedAt");

CREATE UNIQUE INDEX "IX_AspNetUsers_Email" ON "AspNetUsers" ("Email");

CREATE INDEX "IX_AspNetUsers_IsActive" ON "AspNetUsers" ("IsActive");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203151542_EnhancedDatabaseConstraints', '8.0.5');

COMMIT;

