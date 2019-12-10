## Download

https://github.com/thebetioplane/OsuReplayViewer/raw/master/distro/ReplayViewer.exe

### Help

Download the exe, put it into an empty folder and run it, the updater
should download all required files.

Set your paths in the settings.txt file (it will be generated for you).

If at anytime the program crashes, open an issue supplying the text of
the "crash.log" file that is created. There may be some features missing
that existed before, but bear with me I'll try to get it back into the
state that it was before the update.

If you want the program to force an update, delete one of the dlls and
it should go into a repair mode.

### OsuReplayViewer Dec 2019 update

So this project is actually very old and I don't want to touch the
codebase too much. However, during the osu! 20191106 update, they
changed the file format of osu!.db, which of course caused this program
to break and so I felt obligated to fix it.

The main problem is this project used to use Microsoft XNA, which is
obsolete and I didn't have an environment setup to compile, etc. My
solution is to port all the rendering to OpenTK + OpenGL.

This does mean that on top of the osu!.db issues I am fixing some other
weird scaling issues and changing the way checking for updates works
(the old updater was a POS). And while I have a build environment setup
now as well as a way to push updates better, I may fix other issues if
you bring them to my attention through the GitHub issue tracker.

### Compiling

These are the instructions for Visual Studio 2017+ on windows because
that is probably the most common.

(1) clone the repo with git

(2) open the package manager console (from within visual studio)

(3) run `Update-Package` to install ManagedBass and OpenTK

(4) additionally you are going to need the dll files "bass.dll" and
"bass_fx.dll". to be placed in your bin/Debug or bin/Release You can get
them from the developers https://www.un4seen.com/ or they are mirrored
in the "distro" folder.

(5) build compile run

(6) It may be beneficial to put a blank file named `no-update` in your
bin/Debug or bin/Release. This will disable all network activity and
prevent the updater from trying to replace what it thinks are outdated
files.
