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

### Moves: Up/Down
- For the corner orientations of a top/bottom move:
	- $0 \rightarrow 0$
	- $1 \rightarrow 2$
	- $2 \rightarrow 1$

### Moves: Left/Right
- For corner orientations of a left/right move:
	- $0 \rightarrow 1$
	- $1 \rightarrow 0$
	- $2 \rightarrow 2$

### Moves: Front/Back
- For corner orientations of a front/back move:
	- $0 \rightarrow 2$
	- $1 \rightarrow 1$
	- $2 \rightarrow 0$