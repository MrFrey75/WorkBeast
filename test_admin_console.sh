#!/bin/bash

# AdminConsole Test Script
# Tests all functions of the WorkBeast AdminConsole

TEST_DIR="/home/fray/Projects/WorkBeast/src/WorkBeast.AdminConsole"
cd "$TEST_DIR"

echo "=========================================="
echo "WorkBeast AdminConsole Comprehensive Tests"
echo "=========================================="
echo ""

# Function to run a menu option with input
run_test() {
    local test_name=$1
    local menu_input=$2
    
    echo "[TEST] $test_name"
    echo "Input: $menu_input"
    echo "---"
    
    # Send the menu input and exit after
    (echo -e "$menu_input\n6" | timeout 10 dotnet run 2>&1) | head -50
    
    echo ""
    sleep 1
}

# TEST 1: Database Management
echo "========== TEST 1: Database Management =========="
echo ""
echo "[TEST] View Database Statistics (Menu: 1 -> 1)"
(echo -e "1\n1\n8" | timeout 15 dotnet run 2>&1) | grep -A 20 "Statistics\|Exercises\|Body Parts" | head -20
echo ""
sleep 2

echo "[TEST] Apply Pending Migrations (Menu: 1 -> 2)"
(echo -e "1\n2\nNo\n8" | timeout 15 dotnet run 2>&1) | grep -i "migrat\|cancel" | head -10
echo ""
sleep 2

# TEST 2: User Management
echo "========== TEST 2: User Management =========="
echo ""
echo "[TEST] List All Users (Menu: 2 -> 1)"
(echo -e "2\n1\n10" | timeout 15 dotnet run 2>&1) | grep -A 20 "Loading users\|Email\|Active" | head -15
echo ""
sleep 2

# TEST 3: Data Management
echo "========== TEST 3: Data Management =========="
echo ""
echo "[TEST] List Exercises (Menu: 3 -> 1)"
(echo -e "3\n1\n8" | timeout 15 dotnet run 2>&1) | grep -A 20 "Exercises\|Name\|System" | head -15
echo ""
sleep 2

echo "[TEST] List Body Parts (Menu: 3 -> 2)"
(echo -e "3\n2\n8" | timeout 15 dotnet run 2>&1) | grep -A 20 "Body Parts\|Name" | head -15
echo ""
sleep 2

# TEST 4: Configuration Management
echo "========== TEST 4: Configuration Management =========="
echo ""
echo "[TEST] View Current Configuration (Menu: 4 -> 1)"
(echo -e "4\n1\n7" | timeout 15 dotnet run 2>&1) | grep -A 30 "Configuration\|Application\|Database\|Logging" | head -25
echo ""
sleep 2

echo "[TEST] View Configuration File Path (Menu: 4 -> 2)"
(echo -e "4\n2\n7" | timeout 15 dotnet run 2>&1) | grep -i "path\|configuration\|appdata" | head -10
echo ""
sleep 2

# TEST 5: System Information
echo "========== TEST 5: System Information =========="
echo ""
echo "[TEST] Display System Information (Menu: 5)"
(echo -e "5\n6" | timeout 15 dotnet run 2>&1) | grep -A 20 "Property\|Value\|OS\|Version\|Database" | head -20
echo ""
sleep 2

echo "=========================================="
echo "Testing Complete"
echo "=========================================="
