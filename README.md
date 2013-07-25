# Readme - Matryoshka


July 02, 2013
by Zening Qu (quzening@remap.ucla.edu)

## How To Run

1. ccndstart
2. ccndc add / tcp lioncub.metwi.ucla.edu
3. double click Matryoshka20130702.app

## What To See
1. asteroids created & destroyed as player flies around
2. NDN names labeled for player and asteroids

## What Is New
1. render is more smooth
2. added NDN name labels for asteroids and player
3. got rid of thread pool; use one thread and one shared handle


# Control Keys
1. arrows: go left/right, walk back/forth, fly up/down
2. 'a', 'd': go left/right
3. 'w', 's': walk back/forth, speed up/down when flying
4. '1': switch to 1st person view
5. '3': switch to 3rd person view
6. mouse: look around
7. 'n': to view NDN names of {{nothing}, {player}, {player, asteroid in aura}, {player, asteroid in nimbus}}

## Known Issue
1. some NDN names keep floating on the screen before being deleted
2. doll sometimes crashes through asteroids when flying too fast