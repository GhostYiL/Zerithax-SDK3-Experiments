# Zerithax's SDK3 Experiments
This project contains various features I've experimented with involving VR interactions such as measuring hand rotation/position to spawn and interact with various objects within VRChat using U#, which converts written C# into Udon visual scripting language.

I will be updating it as I improve upon existing features or attempt to implement new ones. It can be assumed that any features in this project are in an unfinished state and may be updated or abandoned on a whim, as this is mostly just a half-sane programmer's antics. See below for a demonstration of some of the features I've developed:

### Sword Art Online UI
This feature will probably never stop receiving small edits here and there. As a huge SAO fanboy myself, I've always wanted to get the chance to develop the game's features in my own program. I'm also noticing exactly how un-fun an SAO game would really be in real life, given how much players are forced to live in a very small and cluttered UI like this instead of physically grabbing backpacks, etc. But there's still some small bit of joy that comes with getting the chance to do something like this myself after watching my favorite characters do it all those years ago.
<iframe
  src="https://streamable.com/iji9lo"
  width="70%" height="600px">
  </iframe>

### Hand-Activated Magic Circle
No video for this feature yet! It exists, but I haven't gotten around to asking people to let me record them trying it out. This idea was born when I asked myself how a VR user with current-day limitations (e.g. mostly hand and head movements, no voice-to-text or related without massive effort) would be able to cast magic much like they do in SAO's second halves of its first and second seasons: ALO (Alfheim online).
Magic casting in ALO is a matter of lifting one's hand, then reciting a set of magic words and slowly constructing your spell. I imagined that each word used defines a certain part of the spell you're casting (size, power, speed, area of effect, etc.) so I implemented a similar feature using hand movements instead.
The categories of the spell you cast start in the top-left corner and move counter clockwise: Element, Speed, Projectile Type, Size (and consequentially range). If a player casts their spell without specifying a certain effect in a quadrant, it chooses a default that I've specified in my code. 