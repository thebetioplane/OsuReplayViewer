From this repository: https://github.com/smoogipooo/osu-BMAPI
Modifications from original source:
- SliderObject.cs
	Changed a lot of the slider calculations that didn't work or were outdated
- TimingPoint.cs
	Added property 'SliderBpm' to calculate the map's BPM with the increased slider velocity from inheriting sections
		(Ex: a map with 200 bpm, when inheriting section gives it 1.5x slider speed, that inheriting section will have this property read 300)
- Beatmap.cs
	Changed slider velocity calculation to account for timing point and map BPM in addition to the slider velocity map property
	Added a property that gets the folder that the .osu file is in
- Point2.cs
	Added some additional helper methods for convenience