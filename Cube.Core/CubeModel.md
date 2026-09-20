## Rankings
TOP/BOTTOM: WHITE/YELLOW
FRONT/BACK: GREEN/BLUE
LEFT/RIGHT: ORANGE/RED

### Edges:
- There are 12 edges \[0, 11]
- Each EdgeOrientation is \[0, 1] where $0 \implies \text{orientated}$ and $1 \implies \text{unorientated}$ 
	- e.g of a 0 orientation would be a white green side where white is facing up and green is facing the front (ranking is correct)
	- e.g of a 1 orientation would be a white green side where white is facing the left and green is facing the front (ranking is incorrect)

### Corners
- There are 8 corners \[0, 7]
- Corners have 3 faces so there are 3 possible orientations which would be represented as a number \[0,2]
- 0 will represent the correct orientation, so either the white/yellow face is facing either up or down
- 1 will represent the orientation where white/yellow is facing the front/back
- 2 will represent the orientation where white/yellow is facing either the left/right

### Cube Representation
If we were to hold a rubik's cube where green is the front and white is on top then:

`Corners[0]` would be the white green orange corner, and the rest would go clockwise around the cube. So `Corners[1]` is white blue orange, `Corners[2]` is white blue red and  `Corners[3]` is white green red.

This is also repeated for the bottom corners but starts from 4 instead of 0

As for the Edges, we start at the white green edge, and also go clockwise similar to corners. The only difference here is that the edges also has a middle layer, but, the pattern is also the same on the middle layer.

### Moves: Up/Down
- For the corner orientations of a top/bottom move:
	- $0 \rightarrow 0$
	- $1 \rightarrow 2$
	- $2 \rightarrow 1$

- Edge orientations stay unaffected since the face that is facing up will remain facing up

### Moves: Left/Right
- For corner orientations of a left/right move:
	- $0 \rightarrow 1$
	- $1 \rightarrow 0$
	- $2 \rightarrow 2$

- Edge orientations will also stay the same here even though the front and back will move up a rank, the left and right will still stay on the left and right, and thus still adhering to the ranking established at the start
### Moves: Front/Back
- For corner orientations of a front/back move:
	- $0 \rightarrow 2$
	- $1 \rightarrow 1$
	- $2 \rightarrow 0$

- For edge orientations of a front/back move:
	- $0 \rightarrow 1$
	- $1 \rightarrow 0$