using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace striveClasses
{
    public class GameRun
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        /*
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            Console.Write("\x1b[48;5;" + 250 + "m" + "Text.");


            "\x1b[48;5;" + s + "m" - set background color by index in table (0-255)

            "\x1b[38;5;" + s + "m" - set foreground color by index in table (0-255)

            "\x1b[48;2;" + r + ";" +g+";"+b + "m" - set background by r,g,b values

            "\x1b[38;2;" + r + ";" +g+";"+b + "m" - set foreground by r,g,b values
            */

        //int colors
        public int colorBackground1 = 234;
        public int colorBackground2 = 237;
        public int colorPlayer = 15;
        public int colorWall = 15;
        public int colorEnemy = 210;
        public int colorSelectedBack = 156;
        public int colorSelectedFore = 22;
        public int colorFlashHit = 1;
        //public int colorFlashHit = 160;
        public int colorFlashMiss = 248;

        public string mapFilePath = @"E:\Unity Projects\striveClasses\striveClasses\Map.txt";

        public int gravBootsEnergyCost = 5;

        public static int GetColorBackground1()
        {
            return 234;
        }
        public static int GetColorBackground2()
        {
            return 237;
        }
        public static int GetColorPlayer()
        {
            return 15;
        }
        public static int GetColorEnemy()
        {
            return 210;
        }
        public static int GetColorSelectedBack()
        {
            return 156;
        }
        public static int GetColorSelectedFore()
        {
            return 22;
        }
        public static int GetColorFlashHit()
        {
            return 1;
            //return 160;
        }
        public static int GetColorFlashMiss()
        {
            return 248;
        }

        public int visibility = 15;

        public bool VisibilityDebugMode
        {
            get; set;
        }

        public int GetxPadding()
        {
            return 30;
        }
        public int GetyPadding()
        {
            return 30;
        }
        public int GetHeight()
        {
            return (this.visibility * 2) + 1;
        }
        public int GetWidth()
        {
            return (this.visibility * 2) + 1;
        }

        public GameMap Map
        {
            get; set;
        }

        public int Clock
        {
            get; set;
        }

        public bool Gravity
        {
            get; set;
        }

        public Player Player
        {
            get; set;
        }

        public List<Enemy> Enemies
        {
            get; set;
        }

        public Enemy SelectedTarget
        {
            get; set;
        }

        public Enemy LastAttacked
        {
            get; set;
        }

        public List<string> EventLog
        {
            get; set;
        }

        public void Log(string e)
        {
            this.EventLog.Add(e);
            
            int temp = 0;
            int dif = 0;

            if (this.EventLog.Count <= this.GetWidth() - 1)
                temp = this.EventLog.Count;
            else
            {
                temp = this.GetWidth() - 1;
                dif = this.EventLog.Count - temp;
            }

            for (int i = 0; i < temp; i++)
            {
                Console.SetCursorPosition(this.GetHeight() + 1, i + 1);
                Console.Write(this.EventLog[dif + i] + "      ");
            }
        }

        private static char[,] InitBoard(string[] rows)
        {
            char[,] temp = new char[rows.Length, rows[0].Length];

            for (int i = 0; i < rows.Length; i++)
                for (int j = 0; j < rows[i].Length; j++)
                {
                    temp[i, j] = rows[i][j];
                }
            return temp;
        }

        public GameRun()
        {
            this.VisibilityDebugMode = false;

            //this.Map = new GameMap(this);
            this.Clock = 0;
            this.Gravity = true;

            this.Enemies = new List<Enemy>();

            /*
            string[] str = new string[] {
                "........................................................................................................................",
                ".+-----------------------------------------------+-------------------+-------------------+----------------------------+.",
                ".|-------+          E                            |                   |                   |             +--------------|.",
                ".|---------+                                     +                   |     E             |                +-----------|.",
                ".|-----------+                                                       |                   |                    +-------|.",
                ".|---------+           E                         +                   |                   |                      +-----|.",
                ".|-------+                                       |                   |             E     |                        +---|.",
                ".+-----------------------------------------------+-------+   +-------+-------+   +-------+----------------------------+.",
                ".|                                                                                       |     E                  E  +|.",
                ".|@                                                                                      |  E                     E  +|.",
                ".|                                                                                       |     E                  E  +|.",
                ".+-----------------------------------------------+-------+   +-------+-------+   +-------+----------------------------+.",
                ".|-------+     E                                 |   E          E    |             E     |                        +---|.",
                ".|---------+                                     +                   |                   |                      +-----|.",
                ".|-----------+                                                       |                   |                    +-------|.",
                ".|---------+                                     +                   |             E     |                +-----------|.",
                ".|-------+      E                                |             E     |                E  |             +--------------|.",
                ".+-----------------------------------------------+-------------------+-------------------+----------------------------+.",
                "........................................................................................................................"
            };
            */

            string[] str = GameMap.GameMapFromFile(this.mapFilePath);

            char[,] map = InitBoard(str);

            this.Map = GameMap.GetMapFromText(map, this);

            this.Enemies.Sort();

            this.EventLog = new List<string>();
        }

        public void Run()
        {
            Console.Title = "Strive: Rogue Station";

            Console.CursorVisible = false;

            int height = this.GetHeight() + this.GetyPadding();
            int width = this.GetWidth() + this.GetxPadding();

            if (height > Console.LargestWindowHeight)
            {
                height = Console.LargestWindowHeight - 1;
            }
            if (width > Console.LargestWindowWidth)
            {
                width = Console.LargestWindowWidth - 1;
            }

            Console.SetWindowSize(width , height);

            this.Map.Print(this);

            this.UpdateTurn();

            //the Log name is not updated later, so we only print it once
            string temp = "Log:";
            Console.SetCursorPosition(width - this.GetxPadding() + 1, 0);
            Console.Write(temp);

            this.Player.UpdateHP(this);
            this.Player.UpdateEnergy(this);
            this.Player.UpdateGravBoots(this);

            Player.UpdateWeapon(this, Player.Selected);

            //update/set the distance to player property for each enemy (required for sorting)
            foreach (Enemy e in this.Enemies)
            {
                e.UpdateDistanceToPlayer(this);
            }

            //select closest enemy
            this.SelectEnemy(this.GetClosestEnemy());

            while (this.Player.HP > 0)
            {
                //player turn run
                Player.Turn(this);

                if(this.Player.HP > 0)
                {
                    this.Clock++;
                    this.UpdateTurn();

                    Enemies.Sort();

                    //enemy turn run
                    for (int i = 0; i < this.Enemies.Count; i++)
                    {
                        if (this.Player.HP > 0)
                        {
                            this.Enemies[i].Turn(this);
                        }
                        else
                            break;
                    }
                    if(this.LastAttacked != null && this.LastAttacked.HP > 0 && this.Map.Map[LastAttacked.X, LastAttacked.Y, 0].VisibleToPlayer)
                    {
                        this.SelectEnemy(this.LastAttacked);
                    }
                    else if (this.Enemies.Count > 0)
                    {
                        this.SelectEnemy(this.GetClosestEnemy());
                    }
                }
            }
        }

        public Enemy GetClosestEnemy()
        {
            this.Enemies.Sort();

            if (this.Enemies[0] != null)
                return this.Enemies[0];
            else
                return null;
        }

        public void SelectEnemy(Enemy enemy)
        {
            this.UnselectEnemy(this.SelectedTarget);
            this.SelectedTarget = enemy;

            if(enemy.DistanceFromPlayer < this.Player.Selected.BaseRange)
                enemy.Print(this, this.colorSelectedFore, this.colorSelectedBack);
            else
                enemy.Print(this, this.colorSelectedBack, this.colorSelectedFore);
        }

        public void UnselectEnemy(Enemy enemy)
        {
            if(SelectedTarget != null && SelectedTarget.HP > 0)
                this.SelectedTarget.Print(this);
            this.SelectedTarget = null;
        }

        public void SwitchTarget(int x, GameRun run)
        {
            if(this.Enemies.Count > 1 && run.Map.Map[this.Enemies[0].X, this.Enemies[0].Y, 0].VisibleToPlayer)
            {
                int index = 0;

                if (this.SelectedTarget != null)
                    index = this.Enemies.IndexOf(this.SelectedTarget);

                if(index <= 0)
                {
                    index = Math.Abs(this.Enemies.Count + x) % this.Enemies.Count;
                }
                else
                    index = (index + x) % this.Enemies.Count;

                if(this.Map.Map[this.Enemies[index].X, this.Enemies[index].Y, 0].VisibleToPlayer)
                {
                    this.SelectEnemy(this.Enemies[index]);
                }
                else
                    SwitchTarget(x + (x/Math.Abs(x)), run);
            }
        }
        //update the display of the turn counter
        public void UpdateTurn()
        {
            string temp = "Turn: " + this.Clock;

            Console.SetCursorPosition(this.GetHeight() - temp.Length, this.GetWidth());
            Console.Write(temp);
        }
    }

    public class MapTile
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        //current coordinates
        public int X
        {
            get; set;
        }

        public int Y
        {
            get; set;
        }

        public bool SeenByPlayer
        {
            get; set;
        }

        public bool VisibleToPlayer
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public char Type
        {
            get; set;
        }

        public int Color
        {
            get; set;
        }

        public int ColorB
        {
            get; set;
        }

        public MapTile(int x, int y, char type, int color, int back)
        {
            this.X = x;
            this.Y = y;

            this.Type = type;
            this.Color = color;
            this.ColorB = back;

            this.VisibleToPlayer = false;
            this.SeenByPlayer = false;
        }

        public override string ToString()
        {
            return this.Type.ToString();
        }

        public void Print(GameRun run, int front, int back)
        {
            if (GameMap.GetDistance(this.X, this.Y, run.Player) < run.visibility && run.Map.Map[this.X, this.Y, 0].VisibleToPlayer)
            if (Math.Abs(this.X - run.Player.X) < run.visibility && Math.Abs(this.Y - run.Player.Y) < run.visibility)
                {
                Console.SetCursorPosition(this.Y - (run.Player.Y - run.visibility), this.X - (run.Player.X - run.visibility));

                var handle = GetStdHandle(-11);
                int mode;
                GetConsoleMode(handle, out mode);
                SetConsoleMode(handle, mode | 0x4);

                Console.Write("\x1b[48;5;" + back + "m");
                Console.Write("\x1b[38;5;" + front + "m" + this);

                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
            }
        }

        public void Print(GameRun run)
        {
            this.Print(run, this.Color, this.ColorB);
        }

        public static void Print(MapTile tile, int color, GameRun run)
        {
            if (GameMap.GetDistance(tile.X, tile.Y, run.Player) < run.visibility && run.Map.Map[tile.X, tile.Y, 0].VisibleToPlayer)
            {
                Console.SetCursorPosition(tile.Y - (run.Player.Y - run.visibility), tile.X - (run.Player.X - run.visibility));

                var handle = GetStdHandle(-11);
                int mode;
                GetConsoleMode(handle, out mode);
                SetConsoleMode(handle, mode | 0x4);

                Console.Write("\x1b[48;5;" + tile.ColorB + "m");
                Console.Write("\x1b[38;5;" + color + "m" + tile);

                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
            }
        }

        public static void Print(MapTile tile, GameRun run)
        {
            MapTile.Print(tile, tile.Color, run);
        }

        public static void PrintNull(GameRun run, int x, int y)
        {
            if (GameMap.GetDistance(x, y, run.Player) < run.visibility)
            {
                Console.SetCursorPosition(y - (run.Player.Y - run.visibility), x - (run.Player.X - run.visibility));

                var handle = GetStdHandle(-11);
                int mode;
                GetConsoleMode(handle, out mode);
                SetConsoleMode(handle, mode | 0x4);

                Console.Write("\x1b[48;5;" + 0 + "m");
                Console.Write("\x1b[38;5;" + 0 + "m" + " ");

                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
            }
        }
    }

    public class MapWall : MapTile
    {
        public MapWall(int x, int y, char c, int front, int back) : base(x, y, c, front, back)
        {

        }
    }

    public class MapFloor : MapTile
    {
        public MapFloor(int x, int y, char c, int front, int back) : base(x, y, c, front, back)
        {

        }
    }

    public class SolidWall : MapWall
    {
        public SolidWall(int x, int y, char c, int front, int back) : base(x, y, c, front, back)
        {
            
        }
    }

    public class GameMap
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        public int Xlength
        {
            get; set;
        }

        public int Ylength
        {
            get; set;
        }

        public MapTile[,,] Map
        {
            get; set;
        }
        
        public GameMap(int x, int y)
        {
            this.Xlength = x;
            this.Ylength = y;

            this.Map = new MapTile[x, y, 2];
        }

        public GameMap(MapTile[,,] map)
        {
            this.Xlength = map.GetLength(0);
            this.Ylength = map.GetLength(1);

            this.Map = map;
        }

        public static string[] GameMapFromFile(string filepath)
        {
            return File.ReadAllLines(filepath);
        }

        public static GameMap GetMapFromText(char[,] map, GameRun run)
        {
            //example of input (numbers are there for debugging and would not be in a real input)
            //map must be square
            //\01234567890123456789
            //0....................
            //1.+----------------+.
            //2.|                |.
            //3.|  @    +---+    |.
            //4.|       |...|    |.
            //5.|       |...| E  |.
            //6.|       +---+    |.
            //7.|            E   |.
            //8.+----------------+.
            //9....................

            /*
            . = solidWall
            + = cornerWall
            - = hWall
            | = vWall
            @ = player
            E = enemy
            ' ' = floor
            */

            GameMap m = new GameMap(map.GetLength(0), map.GetLength(1));

            for(int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
                {
                    int colorB = 0;

                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                        colorB = run.colorBackground1;
                    else
                        colorB = run.colorBackground2;

                    switch (map[i,j])
                    {
                        case '.': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); m.Map[i, j, 1] = new SolidWall(i, j, '.', run.colorWall, colorB); break;
                        case ' ': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); m.Map[i, j, 1] = null; break;
                        case '+': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); m.Map[i, j, 1] = new MapWall(i, j, '+', run.colorWall, colorB); break;
                        case '-': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); m.Map[i, j, 1] = new MapWall(i, j, '-', run.colorWall, colorB); break;
                        case '|': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); m.Map[i, j, 1] = new MapWall(i, j, '|', run.colorWall, colorB); break;
                        case '@': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); Player temp = new Player(run); temp.X = i; temp.Y = j; m.Map[i, j, 1] = temp; run.Player = temp;  break;
                        case 'E': m.Map[i, j, 0] = new MapFloor(i, j, ' ', run.colorWall, colorB); Enemy tempEnemy = new Enemy(i, j, run); m.Map[i, j, 1] = tempEnemy; run.Enemies.Add(tempEnemy); break;
                    }
                }
            }
            return m;
        }

        public void RetraceVisibility(GameRun run)
        {
            //set all tile visibleToPlayer to false
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j, 0].VisibleToPlayer = false;

                    if(Map[i, j, 1] != null)
                    {
                        Map[i, j, 1].VisibleToPlayer = false;
                    }
                }
            }
            
            for(int i = 0; i < 360; i++)
            {
                //make line
                float deg = i * Deg2rad();
                int nx = (int)Math.Round(Math.Cos(deg) * run.visibility) + run.Player.X;
                int ny = (int)Math.Round(Math.Sin(deg) * run.visibility) + run.Player.Y;

                int d = (int)GetDistance(nx, ny, run.Player);

                //loop through all tiles
                for(int j = 0; j < d; j++)
                {
                    int tx = (int)Math.Round(LinearERP(run.Player.X, nx, j / (float)d));
                    int ty = (int)Math.Round(LinearERP(run.Player.Y, ny, j / (float)d));

                    if (tx < 0 || tx > run.Map.Xlength)
                        break;
                    if (ty < 0 || ty > run.Map.Ylength)
                        break;

                    if(run.Map.Map[tx, ty, 1] != null && run.Map.Map[tx, ty, 1] is MapWall)
                    {
                        run.Map.Map[tx, ty, 0].VisibleToPlayer = true;
                        Map[tx, ty, 1].VisibleToPlayer = true;
                        break;
                    }

                    run.Map.Map[tx, ty, 0].VisibleToPlayer = true;
                    run.Map.Map[tx, ty, 0].SeenByPlayer = true;

                    if (Map[tx, ty, 1] != null)
                    {
                        Map[tx, ty, 1].VisibleToPlayer = true;
                    }
                }
            }
        }

        public float LinearERP(float start, float end, float t)
        {
            return start + t * (end - start);
        }

        public float Deg2rad()
        {
            return (float)Math.PI / 180;
        }

        public void Print(GameRun run)
        {
            this.RetraceVisibility(run);

            for (int i = run.Player.X - run.visibility; i < run.Player.X + run.visibility; i++)
            {
                for (int j = run.Player.Y - run.visibility; j < run.Player.Y + run.visibility; j++)
                {
                    //if tile is within bounds of map
                    if (i >= 0 && i < this.Xlength && j >= 0 && j < this.Ylength)
                    {
                        //if tile contains solid object [1 != null] and tile is visible to player, print object with floor backgroung color
                        if (Map[i, j, 1] != null && Map[i, j, 0].VisibleToPlayer)
                        {
                            Map[i, j, 1].Print(run, Map[i, j, 1].Color, Map[i, j, 0].ColorB);
                        }
                        //if tile contains no object and is visible to player, print floor
                        else if (Map[i, j, 1] == null && Map[i, j, 0].VisibleToPlayer)
                        {
                            Map[i, j, 0].Print(run);
                        }
                        /*
                        //if tile is wall and is not visible to player but has been seen before, print floor with shifted color
                        else if (Map[i, j, 1] is MapWall && Map[i, j, 0].VisibleToPlayer == false && Map[i, j, 0].SeenByPlayer == true)
                        {
                            Map[i, j, 1].Print(run, (Map[i, j, 1].Color - 5) % 256, (Map[i, j, 0].ColorB - 5) % 256);
                        }*/
                        //if tile is not visible to player and has not been seen before, print empty and square
                        else if (Map[i, j, 0].VisibleToPlayer == false)
                        {
                            MapTile.PrintNull(run, i, j);
                        }
                    }
                    //if tile is not in bounds of map, print empty square
                    else
                        MapTile.PrintNull(run, i, j);
                }
            }
        }

        public void Add(Sentient e)
        {
            this.Map[e.X, e.Y, 1] = e;
        }

        public void Remove(Sentient e, GameRun run)
        {
            this.Map[e.X, e.Y, 0].Print(run);
            this.Map[e.X, e.Y, 1] = null;
        }

        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
                if (!(x1 == x2 && y1 == y2))
                {
                    int x = Math.Abs(x1 - x2);
                    int y = Math.Abs(y1 - y2);

                    return Math.Sqrt((x * x) + (y * y));
                }
                else
                    return 0;
        }

        public static double GetDistance(int x1, int y1, Sentient e)
        {
            return GetDistance(x1, y1, e.X, e.Y);
        }

        public static double GetDistance(Sentient e1, Sentient e2)
        {
            return GetDistance(e1.X, e1.Y, e2.X, e2.Y);
        }
    }

    public abstract class Sentient : MapTile
    {
        //stats (current)
        public int HP
        {
            get; set;
        }

        public int Energy
        {
            get; set;
        }

        public int Speed
        {
            get; set;
        }

        public int BaseSpeed
        {
            get; set;
        }

        public int Accuracy
        {
            get; set;
        }

        public int Dodge
        {
            get; set;
        }

        public bool GravBoots
        {
            get; set;
        }

        //statsMax
        public int HPMax
        {
            get; set;
        }

        public int EnergyMax
        {
            get; set;
        }

        public Item[] ItemInventory
        {
            get; set;
        }

        public Weapon[] WeaponInventory
        {
            get; set;
        }

        public Weapon Selected
        {
            get; set;
        }

        public Sentient(GameRun run) : base(0, 0, 'S', run.colorEnemy, run.colorBackground1)
        {
            this.HP = this.HPMax = 100;
            this.Energy  = this.EnergyMax = 100;
            this.Speed = 3;
            this.Accuracy = 75;
            this.Dodge = 25;

            this.GravBoots = false;

            this.ItemInventory = new Item[10];
            this.WeaponInventory = new Weapon[3];

            this.Name = "sentientName";

            this.WeaponInventory[0] = new MeleeWeapon();
            this.Selected = this.WeaponInventory[0];
        }

        public Sentient(int x, int y, int hpMax, int energyMax, int speed, int accuracy, int dodge, GameRun run) : base(x, y, 'S', run.colorEnemy, run.colorBackground1)
        {
            this.HPMax = hpMax;
            this.HP = hpMax;
            this.EnergyMax = energyMax;
            this.Energy = energyMax;
            this.BaseSpeed = speed;
            this.Speed = speed;
            this.Accuracy = accuracy;
            this.Dodge = dodge;
            this.GravBoots = false;
            this.Type = 'S';
            this.ItemInventory = new Item[10];
            this.WeaponInventory = new Weapon[3];
            this.Name = "sentientName";
            this.WeaponInventory[0] = new MeleeWeapon();
            this.Selected = this.WeaponInventory[0];
        }

        public Sentient(int x, int y, int hp, int hpMax, int energy, int energyMax, int speed, int accuracy, int dodge, bool grav, char type, GameRun run) : base(x, y, type, run.colorEnemy, run.colorBackground1)
        {
            this.HP = hp; this.HPMax = hpMax;
            this.Energy = energy; this.EnergyMax = energyMax;
            this.Speed = speed;
            this.BaseSpeed = speed;
            this.Accuracy = accuracy;
            this.Dodge = dodge;
            this.GravBoots = grav;
            this.Type = type;
            this.ItemInventory = new Item[10];
            this.WeaponInventory = new Weapon[3];
            this.Name = "sentientName";
            this.WeaponInventory[0] = new MeleeWeapon();
            this.Selected = this.WeaponInventory[0];
        }

        public override string ToString()
        {
            return this.Type.ToString();
        }

        public void Flash(int color, int back, GameRun run)
        {
            base.Print(run, color, back);
            System.Threading.Thread.Sleep(150);
            base.Print(run, this.Color, this.ColorB);
            System.Threading.Thread.Sleep(150);
        }

        public void Flash(int color, int back, int sleepTime, GameRun run)
        {
            base.Print(run, color, back);
            System.Threading.Thread.Sleep(sleepTime);
            base.Print(run, this.Color, this.ColorB);
            System.Threading.Thread.Sleep(sleepTime);
        }

        public abstract void TakeDamage(int dmg, Sentient attacking, GameRun run);

        public bool Attack(Sentient target, RangedWeapon weapon, GameRun run)
        {
            if (weapon.ClipCurrent > 0)
            {
                int attacks = weapon.ModeAttacks[weapon.SelectedMode];

                if (weapon.ClipCurrent < attacks)
                    attacks = weapon.ClipCurrent;

                Random rnd = new Random();

                for (int i = 0; i < attacks; i++)
                {
                    if (run.Gravity == false && this.GravBoots == false)
                    {
                        int x = this.X - target.X;
                        int y = this.Y - target.Y;

                        int absx = Math.Abs(x);
                        int absy = Math.Abs(y);

                        int[,] step = new int[1, 2];

                        step[0, 0] = 0;
                        step[0, 1] = 0;

                        if (absx == absy)
                        {
                            step[0, 0] = x / absx;
                            step[0, 1] = y / absy;
                        }
                        else if (absx > absy)
                        {
                            step[0, 0] = x / absx;
                        }
                        else if (absx < absy)
                        {
                            step[0, 1] = y / absy;
                        }

                        this.Move(run, step);
                    }

                    int accuracy = rnd.Next(0, this.Selected.BaseAccuracy);
                    int dodge = rnd.Next(0, target.Dodge);

                    if (accuracy > dodge)
                    {
                        int dmg = this.Selected.BaseDamage;

                        //fix dmg += modifiers 

                        target.TakeDamage(dmg, this, run);
                    }
                    else if(target.HP > 0)
                    {
                        target.Flash(run.colorFlashMiss, run.Map.Map[target.X, target.Y, 0].ColorB, run);

                        run.Log(this.ToString() + " missed " + target.ToString());
                    }
                    //use bullet hit or miss
                    weapon.ClipCurrent--;

                    if(this is Player)
                    {
                        Sentient.UpdateAmmo(run, weapon);
                    }
                }
                return true;
            }
            else
                return false;

        }

        static public void UpdateWeapon(GameRun run, Weapon weapon)
        {
            Console.SetCursorPosition(0, run.GetWidth() + 4);
            Console.Write("                                ");
            Console.SetCursorPosition(0, run.GetWidth() + 5);
            Console.Write("                                ");
            Console.SetCursorPosition(0, run.GetWidth() + 6);
            Console.Write("                                ");
            Console.SetCursorPosition(0, run.GetWidth() + 4);
            Console.Write(weapon.Name + ": ");

            if (weapon is RangedWeapon)
            {
                UpdateAmmo(run, (RangedWeapon)weapon);

                Console.SetCursorPosition(0, run.GetWidth() + 6);
                Console.Write("Mode: " + weapon.ModeNames[weapon.SelectedMode]);
            }
            else if (weapon is MeleeWeapon)
            { 
                Console.SetCursorPosition(0, run.GetWidth() + 5);
                Console.Write("Mode: " + weapon.ModeNames[weapon.SelectedMode]);
            }
        }

        static public void UpdateAmmo(GameRun run, RangedWeapon gun)
        {
            Console.SetCursorPosition(0, run.GetWidth() + 4);
            Console.Write(gun.Name + ": ");

            double temp = (double)gun.ClipCurrent / (double)gun.ClipMax;
            temp *= 10;

            string[] bar = new string[12];

            for (int i = 0; i < bar.Length - 1; i++)
            {
                bar[i + 1] = " ";
            }

            bar[0] = "[";
            bar[11] = "]";

            for (int i = 0; i < temp; i++)
            {
                bar[i + 1] = "|";
            }
            
            Console.SetCursorPosition(0, run.GetWidth() + 5);
            for (int i = 0; i < bar.Length + gun.ClipMax.ToString().Length * 2 + 2; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, run.GetWidth() + 5);
            for (int i = 0; i < bar.Length; i++)
            {
                Console.Write(bar[i]);
            }
            Console.Write(" " + gun.ClipCurrent + "/" + gun.ClipMax);
        }

        public void UpdateEnergy(GameRun run)
        {
            double temp = (double)this.Energy / (double)this.EnergyMax;
            temp *= 10;

            Console.SetCursorPosition(0, run.GetWidth() + 2);
            Console.Write("Energy:");

            string[] bar = new string[12];

            for (int i = 0; i < bar.Length - 1; i++)
            {
                bar[i + 1] = " ";
            }

            bar[0] = "[";
            bar[11] = "]";

            for (int i = 0; i < temp; i++)
            {
                bar[i + 1] = "|";
            }

            Console.SetCursorPosition(0, run.GetWidth() + 3);
            for (int i = 0; i < bar.Length + this.EnergyMax.ToString().Length * 2 + 2; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, run.GetWidth() + 3);
            for (int i = 0; i < bar.Length; i++)
            {
                Console.Write(bar[i]);
            }
            Console.Write(" " + this.Energy + "/" + this.EnergyMax);
        }

        public void UpdateGravBoots(GameRun run)
        {
            string temp = "GravBoots: [off]";
            Console.SetCursorPosition(0, run.GetWidth() + 7);
            if(run.Player.GravBoots)
                temp = "GravBoots: [on] ";
            else
                temp = "GravBoots: [off]";

            Console.Write(temp);
        }

        public void Attack(Sentient target, MeleeWeapon weapon, GameRun run)
        {
            int attacks = weapon.ModeAttacks[weapon.SelectedMode];

            Random rnd = new Random();

            for (int i = 0; i < attacks; i++)
            {
                int accuracy = rnd.Next(0, this.Selected.BaseAccuracy);
                int dodge = rnd.Next(0, target.Dodge);

                if (accuracy > dodge)
                {
                    int dmg = this.Selected.BaseDamage;

                    //fix
                    //add modifiers

                    //dmg += modifiers 

                    target.TakeDamage(dmg, this, run);
                }
                else
                {
                    target.Flash(244, run.Map.Map[target.X, target.Y, 0].ColorB, run);

                    run.Log(this.ToString() + " missed " + target.ToString());
                }
            }
        }

        public void Move(GameRun run, int[,] path)
        {
            //path[i,{0=x/1=y}], should be stored in pairs, first move at [0,0] and [0,1]

            //      [-1,0]
            //[0,-1]  @  [0,1]
            //      [1,0]

            for (int i = 0; i < path.GetLength(0); i++)
            {
                if (run.Map.Map[this.X + path[i,0], this.Y + path[i,1], 1] == null)
                {
                    Sentient temp = this;

                    run.Map.Map[this.X, this.Y, 1] = null;

                    run.Map.Map[this.X, this.Y, 0].Print(run);

                    run.Map.Map[this.X + path[i, 0], this.Y + path[i, 1], 1] = this;

                    this.X += path[i, 0];
                    this.Y += path[i, 1];

                    this.ColorB = run.Map.Map[this.X, this.Y, 0].ColorB;

                    this.Print(run, this.Color, run.Map.Map[this.X, this.Y, 0].ColorB);

                    if(this.VisibleToPlayer)
                        System.Threading.Thread.Sleep(25);
                }
            }

            if (this is Player)
            {
                foreach (Enemy e in run.Enemies)
                {
                    e.UpdateDistanceToPlayer(run);
                }

                run.Enemies.Sort();

                run.Map.Print(run);

                if (run.LastAttacked != null && run.LastAttacked.HP > 0)
                {
                    run.SelectEnemy(run.LastAttacked);
                }
                else if (run.Enemies.Count > 0)
                {
                    Enemy target = run.GetClosestEnemy();
                    run.SelectEnemy(target);
                }
            }
            
            else if (this is Enemy)
            {
                ((Enemy)this).UpdateDistanceToPlayer(run);
            }
        }
    }

    public class Player : Sentient
    {
        public Player(GameRun run) : base(run)
        {
            this.X = 2;
            this.Y = 2;

            this.Type = '@';
            this.Name = "Player";

            this.Color = GameRun.GetColorPlayer();
            this.ColorB = GameRun.GetColorBackground1();

            List<string> modeNames = new List<string>();
            List<int> modeAttacks = new List<int>();

            modeNames.Add("Semi");
            modeAttacks.Add(1);

            modeNames.Add("Auto");
            modeAttacks.Add(5);

            this.WeaponInventory[0] = new MeleeWeapon();
            this.WeaponInventory[1] = new RangedWeapon();
            this.WeaponInventory[2] = new RangedWeapon(50, 10, 90, modeNames, modeAttacks, 1, 30, "AR");
            this.Selected = this.WeaponInventory[2];
        }

        public Player(int x, int y, int hpMax, int energyMax, int speed, int accuracy, int dodge, GameRun run) : base(x, y, hpMax, energyMax, speed, accuracy, dodge, run)
        {
            this.Type = '@';
            this.Name = "Player";
        }

        public Player(int x, int y, int hp, int hpMax, int energy, int energyMax, int speed, int accuracy, int dodge, bool grav, char type, string name, Weapon[] weaponInv, GameRun run) : base(x, y, hp, hpMax, energy, energyMax, speed, accuracy, dodge, grav, type, run)
        {
            this.Name = name;

            this.WeaponInventory = weaponInv;
            this.Selected = this.WeaponInventory[0];
        }

        public void UpdateHP(GameRun run)
        {
            double temp = (double)this.HP / (double)this.HPMax;
            temp *= 10;
            
            Console.SetCursorPosition(0, run.GetWidth() + 0);
            Console.Write("HP:");

            string[] bar = new string[12];

            for (int i = 0; i < bar.Length - 1; i++)
            {
                bar[i + 1] = " ";
            }

            bar[0] = "[";
            bar[11] = "]";

            for (int i = 0; i < temp; i++)
            {
                bar[i + 1] = "|";
            }

            Console.SetCursorPosition(0, run.GetWidth() + 1);
            for (int i = 0; i < bar.Length + this.HPMax.ToString().Length*2 + 2; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, run.GetWidth() + 1);
            for (int i = 0; i < bar.Length; i++)
            {
                Console.Write(bar[i]);
            }
            Console.Write(" " + this.HP + "/" + this.HPMax);
        }
        
        public override void TakeDamage(int dmg, Sentient attacking, GameRun run)
        {
            if (run.Gravity == false && this.GravBoots == false)
            {
                int x = this.X - attacking.X;
                int y = this.Y - attacking.Y;

                int absx = Math.Abs(x);
                int absy = Math.Abs(y);

                int[,] step = new int[1, 2];

                step[0, 0] = 0;
                step[0, 1] = 0;

                if (absx == absy)
                {
                    step[0, 0] = x / absx;
                    step[0, 1] = y / absy;
                }
                else if (absx > absy)
                {
                    step[0, 0] = x / absx;
                }
                else if (absx < absy)
                {
                    step[0, 1] = y / absy;
                }

                this.Move(run, step);
            }

            this.HP -= dmg;

            run.Log(attacking.ToString() + " dealt " + dmg + " to " + this.ToString());

            this.Flash(run.colorFlashHit, run.Map.Map[this.X, this.Y, 0].ColorB, run);

            this.UpdateHP(run);

            if (this.HP <= 0)
            {
                run.Log(this.ToString() + " is dead");

                this.Print(run, run.colorFlashHit, run.Map.Map[this.X, this.Y, 0].ColorB);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition((run.GetHeight() / 2) - (("M O R T I S").Length / 2), run.GetWidth() / 2);
                Console.WriteLine("M O R T I S");
                Console.SetCursorPosition((run.GetHeight() / 2) - (("press any key to exit").Length / 2), run.GetWidth() / 2 + 1);
                Console.WriteLine("press any key to exit");
                Console.ReadKey(true);

                Environment.Exit(0);
            }
            
        }

        public static void Turn(GameRun run)
        {
            //retrace to properly select target while accounting for visibility
            run.Map.RetraceVisibility(run);
            run.Enemies.Sort();

            if (run.LastAttacked != null && run.LastAttacked.HP > 0)
            {
                run.SelectEnemy(run.LastAttacked);
            }
            else if (run.Enemies.Count > 0)
            {
                Enemy target = run.GetClosestEnemy();
                run.SelectEnemy(target);
            }

            ConsoleKey key;

            bool endingAction = false;
            bool validAction = false;

            int speedLeft = run.Player.Speed;
            int[,] path = new int[1, 2];

            while (!endingAction && speedLeft > 0)
            {
                validAction = false;

                while(!validAction)
                {
                    //fix for holding key
                    //get user input for action
                    key = Console.ReadKey(true).Key;

                    //move left
                    if (key == ConsoleKey.A)
                    {
                        if (run.Map.Map[run.Player.X, run.Player.Y - 1, 1] == null)
                        {
                            if (run.Player.GravBoots == true && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                path[0, 0] = 0; path[0, 1] = -1;

                                run.Player.Move(run, path);

                                run.Player.Energy -= run.gravBootsEnergyCost;
                                run.Player.UpdateEnergy(run);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == true && run.Player.GravBoots == false)
                            {
                                path[0, 0] = 0; path[0, 1] = -1;

                                run.Player.Move(run, path);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                run.Log("GravBoots are off.");
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == true && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                                run.Player.GravBoots = false;
                                run.Player.UpdateGravBoots(run);
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                            }
                        }
                    }
                    //move up
                    else if (key == ConsoleKey.W)
                    {
                        if (run.Map.Map[run.Player.X - 1, run.Player.Y, 1] == null)
                        {
                            if (run.Player.GravBoots == true && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                path[0, 0] = -1; path[0, 1] = 0;

                                run.Player.Move(run, path);

                                run.Player.Energy -= run.gravBootsEnergyCost;
                                run.Player.UpdateEnergy(run);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == true && run.Player.GravBoots == false)
                            {
                                path[0, 0] = -1; path[0, 1] = 0;

                                run.Player.Move(run, path);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                run.Log("GravBoots are off.");
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == true && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                                run.Player.GravBoots = false;
                                run.Player.UpdateGravBoots(run);
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                            }
                        }
                    }
                    //move right
                    else if (key == ConsoleKey.D)
                    {
                        if (run.Map.Map[run.Player.X, run.Player.Y + 1, 1] == null)
                        {
                            if (run.Player.GravBoots == true && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                path[0, 0] = 0; path[0, 1] = 1;

                                run.Player.Move(run, path);

                                run.Player.Energy -= run.gravBootsEnergyCost;
                                run.Player.UpdateEnergy(run);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == true && run.Player.GravBoots == false)
                            {
                                path[0, 0] = 0; path[0, 1] = 1;

                                run.Player.Move(run, path);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                run.Log("GravBoots are off.");
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == true && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                                run.Player.GravBoots = false;
                                run.Player.UpdateGravBoots(run);
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                            }
                        }
                    }
                    //move down
                    else if (key == ConsoleKey.S)
                    {
                        if (run.Map.Map[run.Player.X + 1, run.Player.Y, 1] == null)
                        {
                            if(run.Player.GravBoots == true && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                path[0, 0] = 1; path[0, 1] = 0;

                                run.Player.Move(run, path);

                                run.Player.Energy -= run.gravBootsEnergyCost;
                                run.Player.UpdateEnergy(run);

                                speedLeft--;
                                validAction = true;
                            }
                            else if (run.Gravity == true && run.Player.GravBoots == false)
                            {
                                path[0, 0] = 1; path[0, 1] = 0;

                                run.Player.Move(run, path);

                                speedLeft--;
                                validAction = true;
                            }
                            else if(run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy >= run.gravBootsEnergyCost)
                            {
                                run.Log("GravBoots are off.");
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == true && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                                run.Player.GravBoots = false;
                                run.Player.UpdateGravBoots(run);
                            }
                            else if (run.Gravity == false && run.Player.GravBoots == false && run.Player.Energy < run.gravBootsEnergyCost)
                            {
                                run.Log("Out of energy.");
                            }
                        }
                    }
                    //for debugging purposes, toggle gravity
                    else if (key == ConsoleKey.H)
                    {
                        run.Gravity = !run.Gravity;

                        if(run.Gravity)
                            run.Log("Gravity is now on.");
                        else
                            run.Log("Gravity is now off.");

                        validAction = true;
                    }
                    //toggle grav boots
                    else if (key == ConsoleKey.G)
                    {
                        if(run.Player.GravBoots == false && run.Player.Energy > run.gravBootsEnergyCost)
                        {
                            run.Player.GravBoots = true;

                            run.Player.UpdateGravBoots(run);

                            validAction = true;
                        }
                        else if(run.Player.GravBoots == true)
                        {
                            run.Player.GravBoots = false;

                            run.Player.UpdateGravBoots(run);

                            validAction = true;
                        }
                    }
                    //skip turn
                    else if (key == ConsoleKey.X)
                    {
                            validAction = true;
                            endingAction = true;
                    }
                    //attack selected enemy
                    else if (key == ConsoleKey.F)
                    {
                        if (run.Enemies.Count > 0)
                        {
                            if (run.SelectedTarget.DistanceFromPlayer < run.Player.Selected.BaseRange && run.Map.Map[run.SelectedTarget.X, run.SelectedTarget.Y, 0].VisibleToPlayer)
                            {
                                if(run.Player.Selected is RangedWeapon)
                                {
                                    validAction = run.Player.Attack(run.SelectedTarget, (RangedWeapon)run.Player.Selected, run);
                                }
                                else if (run.Player.Selected is MeleeWeapon)
                                {
                                    run.Player.Attack(run.SelectedTarget, (MeleeWeapon)run.Player.Selected, run);
                                    validAction = true;
                                }

                                if(validAction)
                                {
                                    endingAction = true;
                                    if (run.SelectedTarget.HP > 0)
                                        run.LastAttacked = run.SelectedTarget;
                                }
                            }
                        }
                    }
                    //switch target up
                    else if (key == ConsoleKey.E)
                    {
                        run.SwitchTarget(+1, run);
                    }
                    //switch target down
                    else if (key == ConsoleKey.Q)
                    {
                        run.SwitchTarget(-1, run);
                    }
                    //reload
                    else if (key == ConsoleKey.R)
                    {
                        if(run.Player.Selected is RangedWeapon)
                        {
                            RangedWeapon temp = new RangedWeapon((RangedWeapon)run.Player.Selected);

                            if(temp.ClipCurrent < temp.ClipMax)
                            {
                                run.Player.Selected.Reload(run);

                                validAction = true;
                                endingAction = true;
                            }
                        }
                    }
                    //switch firing mode
                    else if (key == ConsoleKey.B)
                    {
                        if (run.Player.Selected is RangedWeapon)
                        {
                            run.Player.Selected.SelectedMode = (run.Player.Selected.SelectedMode + 1) % run.Player.Selected.ModeNames.Count;
                        }
                        Sentient.UpdateWeapon(run, run.Player.Selected);
                    }
                    //switch weapon to slot 1
                    else if (key == ConsoleKey.D1)
                    {
                        if (run.Player.WeaponInventory[0] != null)
                        {
                            run.Player.Selected = run.Player.WeaponInventory[0];
                            Sentient.UpdateWeapon(run, run.Player.Selected);

                            run.SelectEnemy(run.SelectedTarget);
                        }
                    }
                    //switch weapon to slot 2
                    else if (key == ConsoleKey.D2)
                    {
                        if (run.Player.WeaponInventory[1] != null)
                        {
                            run.Player.Selected = run.Player.WeaponInventory[1];
                            Sentient.UpdateWeapon(run, run.Player.Selected);

                            run.SelectEnemy(run.SelectedTarget);
                        }
                    }
                    //switch weapon to slot 3
                    else if (key == ConsoleKey.D3)
                    {
                        if (run.Player.WeaponInventory[2] != null)
                        {
                            run.Player.Selected = run.Player.WeaponInventory[2];
                            Sentient.UpdateWeapon(run, run.Player.Selected);

                            run.SelectEnemy(run.SelectedTarget);
                        }
                    }
                    //exit
                    else if (key == ConsoleKey.Escape)
                    {
                        validAction = true;
                        endingAction = true;
                        Environment.Exit(0);
                    }

                }
            }
        }
    }

    public class Enemy : Sentient, IComparable<Enemy>, IComparer<Enemy>
    {
        public Enemy(int x, int y, int hpMax, int energyMax, int speed, int accuracy, int dodge, GameRun run) : base(x, y, hpMax, energyMax, speed, accuracy, dodge, run)
        {
            this.Type = 'E';

            this.Color = GameRun.GetColorEnemy();
            this.ColorB = GameRun.GetColorBackground1();
        }

        public Enemy(int x, int y, int hp, int hpMax, int energy, int energyMax, int speed, int accuracy, int dodge, bool grav, char type, GameRun run) : base(x, y, hp, hpMax, energy, energyMax, speed, accuracy, dodge, grav, type, run)
        {
            this.Color = GameRun.GetColorEnemy();
            this.ColorB = GameRun.GetColorBackground1();
        }

        public Enemy(int x, int y, int hp, int hpMax, int energy, int energyMax, int speed, int accuracy, int dodge, bool grav, char type, Weapon selected, GameRun run) : this(x, y, hp, hpMax, energy, energyMax, speed, accuracy, dodge, grav, type, run)
        {
            this.Selected = selected;
        }

        public Enemy(int x, int y, GameRun run) : base(run)
        {
            Random rnd = new Random();

            this.X = x; this.Y = y;
            this.HPMax = rnd.Next(50, 100);
            this.HP = this.HPMax;
            this.EnergyMax = rnd.Next(50, 100);
            this.Energy = this.EnergyMax;
            this.BaseSpeed = 2;
            this.Speed = this.BaseSpeed;
            this.Accuracy = rnd.Next(60, 90);
            this.Dodge = rnd.Next(0, 25);
            this.GravBoots = false;
            this.Type = 'E';
            this.ItemInventory = new Item[10];
            this.WeaponInventory = new Weapon[3];
            this.Name = "generic enemy";
            this.WeaponInventory[0] = new MeleeWeapon();
            this.Selected = this.WeaponInventory[0];

            this.Color = GameRun.GetColorEnemy();
            this.ColorB = GameRun.GetColorBackground1();
        }

        public double DistanceFromPlayer
        {
            get; set;
        }

        public void UpdateDistanceToPlayer(GameRun run)
        {
            this.DistanceFromPlayer = GameMap.GetDistance(this, run.Player);
        }

        public override void TakeDamage(int dmg, Sentient attacking, GameRun run)
        {
            if (this.HP > 0)
            {
                if (run.Gravity == false && this.GravBoots == false)
                {
                    int x = this.X - attacking.X;
                    int y = this.Y - attacking.Y;

                    int absx = Math.Abs(x);
                    int absy = Math.Abs(y);

                    int[,] step = new int[1, 2];

                    step[0, 0] = 0;
                    step[0, 1] = 0;

                    if (absx == absy)
                    {
                        step[0, 0] = x / absx;
                        step[0, 1] = y / absy;
                    }
                    else if (absx > absy)
                    {
                        step[0, 0] = x / absx;
                    }
                    else if (absx < absy)
                    {
                        step[0, 1] = y / absy;
                    }

                    this.Move(run, step);
                }

                this.HP -= dmg;

                run.Log(attacking.ToString() + " dealt " + dmg + " to " + this.ToString());

                this.Flash(GameRun.GetColorFlashHit(), run.Map.Map[this.X, this.Y, 0].ColorB, run);

                if (this.HP <= 0)
                {
                    run.Map.Remove(this, run);

                    int index = run.Enemies.IndexOf((Enemy)this);

                    run.Enemies.RemoveAt(index);

                    run.Log(this.ToString() + " is dead");

                    run.Map.Map[this.X, this.Y, 0].Print(run);
                }
            }
        }

        public static List<Enemy> Generate(int n, GameRun run)
        {
            //add dependency on difficulty and room size?

            List<Enemy> l = new List<Enemy>();

            Random rnd = new Random();

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    l.Add(new Enemy(rnd.Next(10, 30), rnd.Next(10, 30), rnd.Next(50, 100), rnd.Next(50, 100), 2, rnd.Next(60, 90), rnd.Next(0, 25), run));
                    l[i].UpdateDistanceToPlayer(run);
                }

                return l;
            }
            else
                return null;
        }

        public void Turn(GameRun run)
        {
            //if player is sensing range of enemy (2*visibility)
            if(this.DistanceFromPlayer < run.visibility * 2)
            {
                bool canMove = true;

                if(run.Gravity == false && this.Energy < run.gravBootsEnergyCost)
                {
                    canMove = false;
                    this.GravBoots = false;
                }
                else if (run.Gravity == false && this.Energy >= run.gravBootsEnergyCost)
                {
                    this.GravBoots = true;
                }
                else if (run.Gravity == true)
                {
                    this.GravBoots = false;
                }

                //if player is in range of weapon, attack
                if (this.VisibleToPlayer && GameMap.GetDistance(run.Player, this) < this.Selected.BaseRange)
                {
                    if (this.Selected is RangedWeapon)
                    {
                        if (this.Attack(run.Player, (RangedWeapon)this.Selected, run) == false)
                            this.Selected.Reload(run);
                    }
                    else if (this.Selected is MeleeWeapon)
                    {
                        this.Attack(run.Player, (MeleeWeapon)this.Selected, run);
                    }
                }
                //else, player is out of range
                else if (this.Speed > 0 && canMove)
                {
                    int[,] path = AStar.getPathToPlayer(run, this);
                    int[,] tempPath = new int[1, 2];

                    int temp = 0;
                    if (path.GetLength(0) < this.Speed)
                        temp = this.Speed;
                    else
                        temp = path.GetLength(0);

                    for (int i = 0; i < temp; i++)
                    {
                        //if in range after step, attack and end turn
                        if (run.Map.Map[this.X, this.Y, 0].VisibleToPlayer && GameMap.GetDistance(run.Player, this) < this.Selected.BaseRange)
                        {
                            if (this.Selected is RangedWeapon)
                            {
                                if (this.Attack(run.Player, (RangedWeapon)this.Selected, run) == false)
                                    this.Selected.Reload(run);
                            }
                            else if (this.Selected is MeleeWeapon)
                            {
                                this.Attack(run.Player, (MeleeWeapon)this.Selected, run);
                            }
                            break;
                        }
                        else
                        {
                            tempPath[0, 0] = path[i, 0];
                            tempPath[0, 1] = path[i, 1];

                            this.Move(run, tempPath);
                            
                            if(run.Gravity == false)
                            {
                                this.Energy -= run.gravBootsEnergyCost;
                            }

                            if (run.Gravity == false && this.Energy < run.gravBootsEnergyCost)
                            {
                                canMove = false;
                                this.GravBoots = false;
                            }
                        }
                    }
                }
            }
        }

        int IComparable<Enemy>.CompareTo(Enemy e)
        {
            if (this.VisibleToPlayer && !e.VisibleToPlayer)
            {
                return -1;
            }
            else if (!this.VisibleToPlayer && e.VisibleToPlayer)
            {
                return 1;
            }
            else //(this.VisibleToPlayer && e.VisibleToPlayer)
            {
                if (this.DistanceFromPlayer > e.DistanceFromPlayer)
                {
                    return 1;
                }
                else if (this.DistanceFromPlayer < e.DistanceFromPlayer)
                {
                    return -1;
                }
                else
                    return 0;
            }
        }
        
        int IComparer<Enemy>.Compare(Enemy e1, Enemy e2)
        {
            if (e1.Selected.BaseDamage > e2.Selected.BaseDamage)
            {
                return 1;
            }
            else if (e1.Selected.BaseDamage < e2.Selected.BaseDamage)
            {
                return -1;
            }
            else
            {
                if (e1.HPMax > e2.HPMax)
                {
                    return 1;
                }
                else if (e1.HPMax < e2.HPMax)
                {
                    return -1;
                }
                else
                return 0;
            }   
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            //import items(weapons included)
            //import classes
            //import enemies
            //import map/generate map

            GameRun run = new GameRun();
            
            run.Run();
    
            Console.ReadKey(true);
        }
    }
}
