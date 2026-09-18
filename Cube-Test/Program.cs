var cube = new Cube.Core.Cube();

Console.WriteLine("Cube before applying U moves");
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying U move");
cube.ApplyMove(Cube.Core.Move.U);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved()); // False

Console.WriteLine("Applying U2 move");
cube.ApplyMove(Cube.Core.Move.U2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved()); // False

Console.WriteLine("Applying U' move");
cube.ApplyMove(Cube.Core.Move.UPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved()); // False

Console.WriteLine("Doing U2 move to solve the cube");
cube.ApplyMove(Cube.Core.Move.U2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved()); // True
