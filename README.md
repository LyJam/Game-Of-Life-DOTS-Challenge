# Game Of Life DOTS Challenge

This project was part of a game jam challenge on Itch.io: https://itch.io/jam/dots-challenge-1 

Video with explanation: https://youtu.be/LP15URd3Y3w

Strategy: 

Every cell is an entity with 2 components:
- a Position
- a boolean value to tell if the cell is alive or dead
  
Each frame 2 main jobs run to calculate the next state of the cells:
- The first job puts the alive state of the cell in a large NativeArray of bytes where 0x00 represents an alive cell and 0xFF represents a dead cell.
- The second jobs iterates through all cells and calculates the amount of neighbors based on the data from the nativearray.

The grid is then displayed to a texture using the LoadRawTextureData method. The texture only holds an 8 bit Alpha channel and has a white texture behind it, so transparent squares (0x00) will be white and non-transparent (0xFF) will be black. This takes advantage of the fact that the array was already calculated in the first job as mentioned above.

Performance (Intel i9-12900; 32GB RAM; GeForce RTX 4060) 
4000x4000 grid: 60fps 
6000x6000 grid: 30fps
