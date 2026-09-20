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

Console.WriteLine("Applying D move");
cube.ApplyMove(Cube.Core.Move.D);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying D2 move");
cube.ApplyMove(Cube.Core.Move.D2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying D' move");
cube.ApplyMove(Cube.Core.Move.DPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Doing D2 to solve the cube");
cube.ApplyMove(Cube.Core.Move.D2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying R move");
cube.ApplyMove(Cube.Core.Move.R);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying R2 move");
cube.ApplyMove(Cube.Core.Move.R2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying R' move");
cube.ApplyMove(Cube.Core.Move.RPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Doing R2 to solve the cube");
cube.ApplyMove(Cube.Core.Move.R2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying L move");
cube.ApplyMove(Cube.Core.Move.L);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying L2 move");
cube.ApplyMove(Cube.Core.Move.L2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying L' move");
cube.ApplyMove(Cube.Core.Move.LPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Doing L2 to solve the cube");
cube.ApplyMove(Cube.Core.Move.L2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying F move");
cube.ApplyMove(Cube.Core.Move.F);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying F2 move");
cube.ApplyMove(Cube.Core.Move.F2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying F' move");
cube.ApplyMove(Cube.Core.Move.FPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Doing F2 to solve the cube");
cube.ApplyMove(Cube.Core.Move.F2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying B move");
cube.ApplyMove(Cube.Core.Move.B);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying B2 move");
cube.ApplyMove(Cube.Core.Move.B2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Applying B' move");
cube.ApplyMove(Cube.Core.Move.BPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Doing B2 to solve the cube");
cube.ApplyMove(Cube.Core.Move.B2);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Testing R U R' U'");
cube.ApplyMove(Cube.Core.Move.R);
cube.ApplyMove(Cube.Core.Move.U);
cube.ApplyMove(Cube.Core.Move.RPrime);
cube.ApplyMove(Cube.Core.Move.UPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());

Console.WriteLine("Reversing the turns (U R U' R')");
cube.ApplyMove(Cube.Core.Move.U);
cube.ApplyMove(Cube.Core.Move.R);
cube.ApplyMove(Cube.Core.Move.UPrime);
cube.ApplyMove(Cube.Core.Move.RPrime);
Console.WriteLine(cube);
Console.WriteLine(cube.IsSolved());
