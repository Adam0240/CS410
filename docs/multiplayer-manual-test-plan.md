<!-- Multiplayer summary: manual two-computer checklist for validating connection flow, shared-world sync, and disconnect behavior. -->
# Multiplayer Manual Test Plan

## Setup

- Build the solution on both machines.
- Launch the host copy first and choose `Host multiplayer`.
- Launch the client copy second and choose `Join multiplayer`.
- Use the host machine's LAN IP and the same port on both sides.

## Core connection flow

1. Host starts the server and waits for one client.
2. Client connects and receives `Connected to host.`.
3. Host console prints that `Player 2 joined.`.
4. Client receives the current shared-world snapshot and room text.

## Movement and presence

1. Move Player 1 and Player 2 independently through different rooms.
2. Confirm neither side blocks waiting for the other player's turn.
3. Bring both players into the same room.
4. Confirm each side sees a presence line for the other player.
5. Separate the players again and confirm the presence line disappears.

## Shared world sync

1. Move both players to the battleground.
2. Have Player 1 run `take axe`.
3. Confirm Player 2 no longer sees the axe in the room.
4. Have Player 1 run `drop axe`.
5. Confirm Player 2 now sees the axe in the room.
6. Move both players to the swamp with the axe in Player 1 inventory.
7. Have Player 1 run `use axe`.
8. Confirm Player 2 sees the cleared-swamp description with the hidden path.

## Stability checks

1. Enter an invalid movement command such as `go west` from camp and confirm the game stays responsive.
2. Enter an invalid item command such as `take sword` in a room without that item and confirm the game stays responsive.
3. Try `save`, `load`, and `delete` during multiplayer and confirm each prints that save/load is unavailable.
4. Disconnect the client and confirm the host prints that `Player 2 disconnected.`.
5. Shut down the host while the client is connected and confirm the client prints a host shutdown/disconnect message without crashing.
