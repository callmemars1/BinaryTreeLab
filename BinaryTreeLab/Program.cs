using BinaryTreeLab;

var map = new Map<int, int>();
for (int i = 0; i < 100; ++i) 
{
    map.TryAdd(i, i);
}
Console.WriteLine("=======================================================================");
foreach (var e in map.GetElements()) 
{
    Console.WriteLine(e.key + "\t" + e.value);
}
Console.WriteLine("=======================================================================");
for (int i = 0; i < 100; ++i)
{
    Console.WriteLine(map[i]);
}
Console.WriteLine("=======================================================================");
for (int i = 0; i < 100; ++i)
{
    map.TryRemove(i, out var pair);
    Console.WriteLine("removed: " + pair.Value.ToString());
}
foreach (var e in map.GetElements())
{
    Console.WriteLine(e.key + "\t" + e.value);
}