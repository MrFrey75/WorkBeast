#!/bin/bash
cd /home/fray/Projects/WorkBeast/src/WorkBeast.AdminConsole

# Test: Database Statistics
echo "========== TEST: View Database Statistics =========="
{
    sleep 1
    echo -ne "\033[B"  # Down arrow
    sleep 0.5
    echo -ne "\n"  # Enter
    sleep 3
    echo -ne "\033[B"  # Down arrow to Back
    for i in {1..7}; do echo -ne "\033[B"; done
    sleep 0.5
    echo -ne "\n"  # Exit
    sleep 1
} | dotnet run 2>&1 | tail -50

echo ""
echo "========== TEST COMPLETED =========="
