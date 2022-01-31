# Resolution customiser for Impressions' City Building series
The various isometric 2D city building games that were created by Impressions Games were hardcoded to only allow setting specific resolution options. These were 640x480, 800x600 and 1024x768; with Zeus and Emperor dropping the 480p option and only allowing the other two. Though this was manageable at the time due to these being the most common resolutions used then, screens have since gotten bigger and denser. The switch to LCD further caused issues due to their blurring images that run outside their native resolutions, as well as the adoption of widescreen aspect ratios meaning that 4:3 resolutions are stretched and distorted.

This started to change on [the 5th of February, 2012](https://www.wsgf.org/phpBB3/viewtopic.php?p=131597#p131597) when a Widescreen Gaming Forum member named JackFuste uploaded copies of Pharaoh's EXE that he had modified to run on a few widescreen resolutions. From there, he went on to upload a number of packs and fixes containing edits for all 4 games to run on a number of common resolutions and whatever resolutions users requested. While a massive improvement over the default options, the main issue was that Jack was for the most part the only one making these fixes and any resolution not already available had to be requested. Another thing that can be seen is that he became better at modifying the resolutions over time, ultimately adding some fixes to the UI in his latest mods that weren't present in earlier versions. This meant that his fixes as they stand have varying levels of quality and some resolution options simply never had fixes created.

A small area of progress here came from another Widescreen Gaming Forum member named Mario who, [on the 4th of September, 2018](https://www.wsgf.org/phpBB3/viewtopic.php?p=172910#p172910), uploaded his own modifications for Zeus and Emperor that came with a guide describing how he created his fixes. With this guide, it was now easier for people to create their own fixes.

And that is where this project comes in. By using Mario's guides as well as comparing JackFuste's modified EXEs with the originals, I have managed to start creating programs that will allow a user to create their own versions of the games that will run on a much wider range of resolutions than was possible with even my widescreen fixes packs (which packages together all of the widescreen fixes that JackFuste created) and with all of the fixes that are present in the latest versions of his patches. I would also like to add useful EXE patches to this project so that they can be applied in addition to the resolution changes. One example of this is [the animation fix patch for Zeus created by Pecunia](https://www.wsgf.org/phpBB3/viewtopic.php?p=172300#p172300).

The ultimate aim of this project is to support adapting as many different versions of the various games (i.e. languages and distribution channels) as possible to use almost any resolution the user desires (within reason).

# Contributions
One way that anyone can contribute to the development of this project is by telling me where I can find copies of the games that these patchers don't support yet. For example, non-english versions of these games are one thing I really want to support but which are very difficult to find.

More knowledgable users can contribute by giving me a list of offsets that need to be patched. You can find a list of the things that need patching by opening a utility's `<game-name>_ExeDefinitions` class then look at the `ResHexOffsetTable` struct to get a list with fairly descriptive names. The `FillResHexOffsetTable` function is used to define what offsets are used for which version of the game for resolution editing. The `GetAndCheckExeChecksum` function is used to figure out which version of the game the player is using and the other functions in this class are for defining the offsets for any other patches available. You could even make the required changes to this class yourself and submit a Pull Request. Keep in mind that I will only accept such a PR if I can verify your changes against the version of the game you are targeting. Your PR also needs to comply with other requirements I outlined in the FAQ section.

Finally, though I don't do this with the intention of getting money and my work here will always be free, any financial contributions will be greatly appreciated. You can use the list of payment services under **Sponsor this project** to do so. Otherwise, please see the readmes for the various utilities for other ways you can do so, including "free":
- [Emperor Resolution Customiser readme](https://xjdhdr.gitlab.io/my_creations/Impressions_Resolution_Customiser/Emperor_Resolution_Customiser.html)
- [Zeus & Poseidon Resolution Customiser readme](https://xjdhdr.gitlab.io/my_creations/Impressions_Resolution_Customiser/Zeus_Poseidon_Resolution_Customiser.html)

# FAQ
**What is your current progress?**
- Right now, the Zeus & Poseidon Customiser is pretty much feature complete but only supports the Steam and GOG versions of the game. The only work that really still needs to be done is to add support for additional languages and distributions of the game.
- The Emperor Customiser is mostly finished in terms of supporting the GOG version of the game. It still needs some work and testing before I can start releasing it.

**Where are the binaries/files that players can use?**
- The binaries are uploaded to Nexus Mods and can be downloaded from there. Please see the readmes above to find links to these files.
- This repository only holds the source code for my utilities. The various releases only mark the state of the repository after releasing a new version of a program.

**What are your future plans?**
- For Zeus and Poseidon, just adding support for additional versions of the game to my utility whenever I gain access to those versions.
- In the meantime, I am working on finishing the Emperor Customiser in terms of adding GOG version support. After that, I plan on adding additional versions to that as well.
- Finally, I would also like to create a Resolution Customiser for Pharaoh and Cleopatra. I haven't started on this yet though.

**What about Caesar 3?**
- I might make a resolution patcher for that game as well. This would mostly be for completion sake though. The [Julius](https://github.com/bvschaik/julius) engine replacement pretty much removes the need for any patchers I could make.

**What requirements do you have for adding support for unrecognised versions of the game?**
- There are three requirements:
- First, I am only willing to add support for fully patched copies of the games. Digital distributions of the game are already patched. If you're using a CD version, you can download the relevant patches from here: [Emperor](https://emperor.heavengames.com/downloads/lister.php?category=patches), [Zeus & Poseidon](https://zeus.heavengames.com/downloads/lister.php?category=patches), [Pharaoh & Cleopatra](https://pharaoh.heavengames.com/downloads/lister.php?category=patches) and [Caesar 3](https://caesar3.heavengames.com/downloads/lister.php?category=patches).
- Next, the game's EXE must not be modified in any way. In particular, I will not add support for no-CD patches under any circumstances. If you are using one, remove it. If you are using some other type of modification to the EXE, I recommend providing this resolution customiser an unmodified copy of the game then re-apply your modifications to the modified EXE that this program creates. If this modification is good, you could even suggest that it be added to my program.
- Finally, I need to know where I can find this version of the game. This is because I need to be able to take a look at the game's files to find out at what offsets they need to be modified and test the modifications.

**Why don't you support no-CD patches?**
- The biggest reason is because these patches are used to facilitate piracy, which is [both immoral and damages PC gaming](https://xjdhdr.gitlab.io/tweakguides/system_guides/Piracy/Piracy_1.xhtml). People who disagree will reply with things like: "I only pirate games to try them out", "games are too expensive", "I would never have paid to begin with", "game devs are greedy", "DRM is useless", "only paying customers suffer from DRM" and so on. To those people: Don't bother. I've heard them all and none of them stand up to scrutiny.
- Next, even if we remove that aspect from the discussion, there is no way I can know what exactly was changed in the EXE to remove the copy protection or what bugs and incompatibilities this removal potentially caused. Additionally, there could be any number of variations of these patches where different groups would remove the copy protection in their own way, leading to different code layouts and hence, different offsets that would need to be found.
- In short, the only legitimate reason I can see for using a no-CD patch is if the copy protection has stopped working on modern PCs, meaning that the game can't be played at all without one. This is not the case as far as I'm aware. If you are desperate to play this game without any DRM present, the only way I'm willing to support is to buy the GOG version.

**The game's display menu option for selecting my resolution still says "1024x768".**
- This is only a cosmetic issue. Selecting this option will run the game at your chosen resolution despite what that text says. Unfortunately, I don't know what part of the game needs to be edited to change the text in that option and I don't know anyone who managed to work this out themselves. If you find out where the game needs to be edited to correct this, please do let me know.

