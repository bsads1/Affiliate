#!/bin/bash

# Specify the directory path
directory="/Users/vinhtran/RiderProjects/Affiliate"

# Change to the specified directory
cd "$directory" || exit

# Delete all .sh files in the directory except clean.sh
find . -type f -name "*.sh" ! -name "clean.sh" -delete

echo "All .sh files in $directory, except clean.sh, have been deleted."