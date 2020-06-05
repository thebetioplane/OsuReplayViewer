## What is the manifest file?

The manifest file is how the updater knows which files are out of date.
It is a binary format that contains a version (32 bit integer) and then
a list of required file names and their MD5 hashes.

You have a local manifest in your installation of the program and one is
found on Github. The updater first checks if the version integer matches
on the local and web version and if they do does not do anything else.
Otherwise the outdated or missing files are downloaded based on the MD5
hashes.

## Using the manifest generator

The code is like 70 lines long so the best documentation would be
reading Program.cs.

It looks for a file named "filelist.txt" in your current directory which
is composed of a the version (32 integer) followed by a list of files to
produce the MD5 hashes for and include in the manifest. The binary
"manifest" file is produced.

See the current manifest and "filelist.txt" in the distro folder.
