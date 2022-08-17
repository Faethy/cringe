using System;

namespace _01_Mile_Project
{
    class Stats
    {
        //Player & Enemy Creation.
        public string Name;
        public int Health, Mana, Attack, Defence, CritChance;

        public Stats() { }

        public Stats(string N, int H, int M, int A, int D, int CC)
        {
            Name = N;
            Health = H;
            Mana = M;
            Attack = A;
            Defence = D;
            CritChance = CC;
        }


    }// Enemy & Player Class.
    internal class Program
    {
        static Random RNG = new();
        //Name, HP, MP, ATK, DEF, DC, CC.
        static Stats EnemyTemplate = new();
        static Stats PlayerTemplate = new("", 30, 10, 5, 0, 20);

        static void Main(string[] args)
        {
            // Initalization of needed variables & objects.
            Stats Player = new();
            Stats Enemy = new();
            int Score = 0, TurnCount, DOT, DOTDamage = 3;
            int EnemyAction, EnemyActionRNG, EnemyHealthPriority;
            string Action;

            //Introduction. 
            Console.Write("Enter player name : ");
            Player.Name = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("You are {0}, a knight trapped in a dungeon. Fight your way out, if you can.", Player.Name);
            Console.ReadKey();

            //Hardcoding is bad part 1
            Player.Name = PlayerTemplate.Name;
            Player.Health = PlayerTemplate.Health;
            Player.Mana = PlayerTemplate.Mana;
            Player.Attack = PlayerTemplate.Attack;
            Player.Defence = PlayerTemplate.Defence;
            Player.CritChance = PlayerTemplate.CritChance;

        //Mob Initializing
        MobInitializing:
            Console.Clear();
            MobSelector(RNG.Next(0, 2));

            // Hardcoding is bad part 2 electric boogaloo
            Enemy.Name = EnemyTemplate.Name;
            Enemy.Health = EnemyTemplate.Health;
            Enemy.Attack = EnemyTemplate.Attack;
            Enemy.Defence = EnemyTemplate.Defence;
            Enemy.CritChance = EnemyTemplate.CritChance;

            //Reset Stats after a fight
            Player.Attack = PlayerTemplate.Attack;
            Player.Defence = PlayerTemplate.Defence;
            DOT = 0;
            TurnCount = 1;
            
            //Combat, start!
            Turn:
            Console.Clear();
            Console.WriteLine("PLAYER STATUS : {0} \nHEALTH : {1}   MANA : {2}\nATK : {3}      %CRIT% : {4}\n\nENEMY : {5}\nHP : {6}\n\nWhat do you wish to do?\n\nAttack          Skills\n\nDefend            Quit\n\nEnter the action you wish to perform.\n", Player.Name, Player.Health, Player.Mana, Player.Attack, Player.CritChance, Enemy.Name, Enemy.Health);
            
            //Player's turn
            Action = Console.ReadLine();
            switch (Action)
            {
                //Attack.
                case "attack":
                case "Attack":
                    Enemy.Health = PlayerAttack(Player.Attack, Player.CritChance, Enemy.Health, Enemy.Name);                    
                    break;

                //Defend.
                case "defend":
                case "Defend":
                    Player.Defence = PlayerDefend(Player.Defence);                    
                    break;

                //Skills. Calling a method here is a lot less clean than just keeping it in Main(), so here it is.
                case "skill":
                case "Skill":
                case "skills":
                case "Skills":
                    Skill:
                    if (Player.Mana < 5)
                    {
                        Console.WriteLine("You do not have enough mana to cast any skills.");
                        Console.ReadKey();
                        goto Turn;
                    }
                    else
                    {
                        Console.WriteLine("\nPlease select a skill to use.\n\nMPRecovery   Heal\nAttackUP    Poison");
                        string SkillAction = Console.ReadLine();
                        switch (SkillAction)
                        {
                            case "MPRecovery":
                            case "mprecovery":
                                Console.WriteLine("You recovered some MP.");
                                Player.Mana += 5;
                                Player.Mana = ManaCheck(Player.Mana);
                                break;

                            case "attackup":
                            case "AttackUP":
                                Console.WriteLine("You raised your attack.");
                                Player.Attack += 5;
                                Player.Mana -= 5;
                                break;

                            case "heal":
                            case "Heal":
                                Console.WriteLine("You healed.");
                                Player.Health += 10;
                                Player.Health = HealthCheck(Player.Health);
                                Player.Mana -= 5;
                                break;

                            case "poison":
                            case "Poison":
                                Console.WriteLine("You poisoned the {0}!", Enemy.Name);
                                DOT = 1;
                                Player.Mana -= 5;
                                break;

                            default:
                                Console.WriteLine("Please enter one of the four skills.");
                                goto Skill;
                        }
                    }
                    break;

                // Quit.
                case "quit":
                case "Quit":
                    Console.WriteLine("Your final score is : {0}", Score);
                    Console.ReadKey();
                    Environment.Exit(0);
                    break;

                default:
                    PlayerDeath(Score);
                    break;
            }

            Console.ReadKey();

            //Enemy Turn
            if (Enemy.Health <= 0) //Death check, I can't use goto statement if I made this into a method. 
            {
                Console.WriteLine("{0} has died.", Enemy.Name);
                goto EnemyDeath;
            }
            Enemy.Health = EnemyPoisonCheck(DOT, DOTDamage, Enemy.Health, Enemy.Name); 
            if (Enemy.Health <= 0) //Death check again, 
            {
                Console.WriteLine("{0} has died.", Enemy.Name);
                goto EnemyDeath;
            }
            EnemyHealthPriority = EnemyHealthPriorityCheck(Enemy.Health, EnemyTemplate.Health);
            EnemyActionRNG = RNG.Next(0, 5);
            EnemyAction = EnemyActionSelector(EnemyHealthPriority);
            switch (EnemyAction)
            {
                case 1: //Attack
                case 2:
                    Player.Health = EnemyAttack(EnemyActionRNG, Enemy.Name, Enemy.Attack, Enemy.CritChance, Player.Health, Player.Defence);
                    break;

                case 3: //Heal
                    Enemy.Health = EnemyHeal(EnemyActionRNG, Enemy.Name, Enemy.Health);
                    break;
            }
            if (Player.Health <= 0)
            {
                Console.WriteLine("You died!");
                PlayerDeath(Score);
            }
            Console.ReadKey();
            Console.WriteLine("\nTurn: "+(++TurnCount));
            Console.ReadKey();
            Player.Defence = PlayerTemplate.Defence;
            goto Turn;

            EnemyDeath:
            Console.ReadKey();
            Score += 100 / TurnCount;
            Console.WriteLine("Score: " + Score);

            RecoverySelect:
            Console.Write("What would you like to recover, Health or Mana?"); //Less optimized if I were to use method.
            string Recovery = Console.ReadLine();
            switch (Recovery)
            {
                case "health":
                case "Health":
                    Player.Health += 5;
                    Player.Health = HealthCheck(Player.Health);
                    break;

                case "mana":
                case "Mana":
                    Player.Mana += 5;
                    Player.Mana = ManaCheck(Player.Mana);
                    break;

                default:
                    Console.WriteLine("Please select one of the two.");
                    Console.ReadKey();
                    goto RecoverySelect;
            }

            goto MobInitializing; 
        }    
        static void MobSelector(int MobName) //Selects a Mob for the Player to fight. 
        {           
            string[] MobNames = { "Zombie", "Skeleton", "Witch" };
            string Mob = MobNames[MobName];
            Console.WriteLine("A {0} appears!", Mob);
            Console.ReadKey();

            switch (Mob)
            {
                case "Zombie":
                    EnemyTemplate.Name = "Zombie";
                    EnemyTemplate.Health = 15;
                    EnemyTemplate.Attack = 3;
                    EnemyTemplate.Defence = 3;
                    EnemyTemplate.CritChance = 5;
                    break;
                    
                case "Skeleton":
                    EnemyTemplate.Name = "Skeleton";
                    EnemyTemplate.Health = 7;
                    EnemyTemplate.Attack = 3;
                    EnemyTemplate.Defence = 0;
                    EnemyTemplate.CritChance = 15;
                    break;

                case "Witch":
                    EnemyTemplate.Name = "Witch";
                    EnemyTemplate.Health = 30;
                    EnemyTemplate.Attack = 2;
                    EnemyTemplate.Defence = 0;
                    EnemyTemplate.CritChance = 50;
                    break;
            }
        }
        static int PlayerAttack(int Pl_ATK, int Pl_CRIT, int En_HP, string En_Name) //Player Attack Action.
        {
            if (RNG.Next(1, 100) < Pl_CRIT)
            {
                Pl_ATK *= 2;
                Console.WriteLine("\nCritical hit!");
            }
            Console.WriteLine("you attacked the {0}, dealing {1} damage.", En_Name, Pl_ATK);
            return En_HP -= Pl_ATK;
        }       
        static int PlayerDefend(int Pl_DEF) //Player Defend Action.
        {
            Console.WriteLine("you raise your shield. defence increased.");
            return Pl_DEF += 5;
        }
        static int HealthCheck(int Pl_HP) //Check if Player is over the HP cap.
        {
            if (Pl_HP > PlayerTemplate.Health)
                Pl_HP = PlayerTemplate.Health;
            return Pl_HP;
        }
        static int ManaCheck(int Pl_MP) //Check if Player is over the MP cap.
        {
            if (Pl_MP > PlayerTemplate.Mana)
                Pl_MP = PlayerTemplate.Mana;
            return Pl_MP;
        }
        static int EnemyPoisonCheck(int En_DOT,int En_DOTDamage, int En_HP, string En_Name) //Check if Enemy should take DOT.
        {
            if (En_DOT == 1)
            {
                En_HP -= En_DOTDamage;
                Console.WriteLine("{0} is poisoned, taking {1} damage.", En_Name, En_DOTDamage);
            }
            return En_HP;
        }
        static int EnemyHealthPriorityCheck(int En_HP, int En_HPCON) //Check if Enemy should heal or not.
        {

            if (En_HP == En_HPCON) //won't heal at full health.
            {
                return 1;
            }
            else if (En_HP < En_HPCON / 2) //50% chance to heal when below full health.
            {
                return RNG.Next(1, 2);
            }
            else
                return 0;
        }
        static int EnemyActionSelector(int En_HPPriority) //Choose what the Enemy does.
        {

            if (En_HPPriority == 1) //Won't heal.
            {
                return RNG.Next(1, 2);
            }
            else if (En_HPPriority == 2) //Guaranteed heal. 
            {
                return 3;
            }
            else 
                return RNG.Next(1, 3);
        }
        static int EnemyAttack(int En_ActionRNG,string En_Name, int En_ATK, int En_CC, int Pl_HP, int Pl_DEF) //Enemy Attack Action.
        {
            Console.WriteLine();
            Console.WriteLine("{0} attacks!", En_Name);
            Console.ReadKey();
            if (RNG.Next(1, 100) < En_CC)
            {
                En_ATK *= 2;
                Console.WriteLine("Critical hit!");
                Console.ReadKey();
            }
            En_ATK += (En_ActionRNG - Pl_DEF);
            if (En_ATK <= 0) 
                En_ATK = 0;
            Console.WriteLine("You took {0} damage.", En_ATK);
            return Pl_HP -= En_ATK;
        }
        static int EnemyHeal(int En_ActionRNG, string En_Name, int En_HP) //Enemy Heal Action.
        {
            Console.WriteLine("{0} healed, regaining {1} health.", En_Name, En_ActionRNG);
            return (En_HP + En_ActionRNG);

        }
        static void PlayerDeath(int Pl_Score) //End the game.
        {
            Console.WriteLine("Final Score : {0}", Pl_Score);
            Console.ReadKey();
            Environment.Exit(0);
        }  
    }
}
