#!/bin/bash
# Copyright (c) 2018 Bill Adams. All Rights Reserved.
# Bill Adams licenses this file to you under the MIT license.
# See the license.txt file in the project root for more information.

#Make sure we are working in our source directory
cd $(dirname $BASH_SOURCE)


for DIR in src/*.*; do
	echo "Cleaning $DIR"	
	rm -rfv "$DIR/obj" "$DIR/bin" "$DIR/BenchmarkDotNet.Artifacts"
done

echo "Done Cleaning"

