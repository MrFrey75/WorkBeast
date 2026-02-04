#!/bin/bash
echo "=== WorkBeast First-Time Setup ==="
echo ""
echo "This script will initialize the database."
echo ""

# Get database path
DB_PATH="$HOME/.local/share/WorkBeast/workbeast.db"

if [ -f "$DB_PATH" ]; then
    echo "Database already exists at: $DB_PATH"
    echo "Do you want to delete it and start fresh? (y/N)"
    read -r response
    if [[ "$response" =~ ^[Yy]$ ]]; then
        rm -f "$DB_PATH"
        echo "Database deleted."
    else
        echo "Keeping existing database."
        exit 0
    fi
fi

echo ""
echo "Step 1: Running migrations..."
dotnet ef database update --project ../WorkBeast.Data/WorkBeast.Data.csproj --startup-project ../WorkBeast.Api/WorkBeast.Api.csproj

if [ $? -eq 0 ]; then
    echo "✓ Migrations applied successfully!"
else
    echo "✗ Migration failed!"
    exit 1
fi

echo ""
echo "Setup complete! Database created at: $DB_PATH"
echo ""
echo "You can now run the admin console with: dotnet run"
