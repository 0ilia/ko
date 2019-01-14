using System;

namespace id
{
    class Ip
    {
        private string[] ip;
        public Ip(string _ip)
        {
            this.ip = new string[4];
            this.ip = Ip.Create(_ip);
        }
        public int[] IpA
        {
            get
            {
                int[] arr = new int[4];

                arr[0] = Convert.ToInt32(this.ip[0], 2);
                arr[1] = Convert.ToInt32(this.ip[1], 2);
                arr[2] = Convert.ToInt32(this.ip[2], 2);
                arr[3] = Convert.ToInt32(this.ip[3], 2);
                return arr;
            }
        }
        public void Print()
        {
            Console.WriteLine("{0}.{1}.{2}.{3}",
                this.ip[0],
                this.ip[1],
                this.ip[2],
                this.ip[3]
                );
        }
        public static string[] Create(string _ip)
        {
            string[] ip = new string[4];
            int pos = _ip.IndexOf('.');
            ip[0] = Convert.ToString(_ip.Substring(0, pos));
            _ip = _ip.Substring(pos + 1);
            pos = _ip.IndexOf('.');
            ip[1] = Convert.ToString(_ip.Substring(0, pos));
            _ip = _ip.Substring(pos + 1);
            pos = _ip.IndexOf('.');
            ip[2] = Convert.ToString(_ip.Substring(0, pos));
            _ip = _ip.Substring(pos + 1);
            ip[3] = Convert.ToString(_ip.Substring(0));
            return ip;
        }
        public static bool CheckIp(string ip)
        {
            int[] values = new int[4];
            int pos = ip.IndexOf('.');
            try
            {
                values[0] = Convert.ToInt32(ip.Substring(0, pos));
                ip = ip.Substring(pos + 1);
                pos = ip.IndexOf('.');
                values[1] = Convert.ToInt32(ip.Substring(0, pos));
                ip = ip.Substring(pos + 1);
                pos = ip.IndexOf('.');
                values[2] = Convert.ToInt32(ip.Substring(0, pos));
                ip = ip.Substring(pos + 1);
                values[3] = Convert.ToInt32(ip.Substring(0));
            }
            catch (Exception e)
            {
                return false;
            }
            foreach (int item in values)
            {
                if (item > 255 || item < 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool CheckMask(string mask)
        {
            if (!Ip.CheckIp(mask))
            {
                return false;
            }
            else
            {
                string[] maskItem = Ip.Create(mask);
                string[] maskItemBi = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    maskItemBi[i] = Convert.ToString(Convert.ToInt32(maskItem[i]), 2);
                    if (maskItemBi[i].Length < 8)
                    {
                        int length = maskItemBi[i].Length;
                        for (int j = 0; j < (8 - length); j++)
                        {
                            maskItemBi[i] = "0" + maskItemBi[i];
                        }
                    }
                }
                string fullMask = "";
                foreach (string item in maskItemBi)
                {
                    fullMask += item;
                }
                int count = 0;
                if (fullMask[0] != '1')
                {
                    return false;
                }
                else
                {
                    for (int i = 1; i < fullMask.Length; i++)
                    {
                        if (fullMask[i - 1] != fullMask[i])
                        {
                            count++;
                        }
                    }
                    if (count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
    class Network
    {
        private Ip ip;
        private Ip mask;
        private Ip networkId;
        private Ip hostId;
        private Ip broadcast;
        private bool check;
        public Network(string _ip, string _mask)
        {
            if (!Ip.CheckIp(_ip) || !Ip.CheckIp(_mask))
            {
                this.check = false;
                Console.WriteLine("Error in your ip addres or mask syntax!");
            }
            else if (!Ip.CheckMask(_mask))
            {
                this.check = false;
                Console.WriteLine("Incorrect mask!");
            }
            else
            {
                this.check = true;
                this.ip = new Ip(_ip);
                this.mask = new Ip(_mask);
                this.networkId = new Ip(Network.GetNetworkId(_ip, _mask));
                this.hostId = new Ip(Network.GetHostId(_ip, _mask));
                this.broadcast = new Ip(Network.GetBroadsast(_ip, _mask));
            }
        }
        public void Print()
        {
            if (!this.check)
            {
                return;
            }
            else
            {
                Console.WriteLine();
                Console.Write("ip: ");
                this.ip.Print();
                Console.Write("mask: ");
                this.mask.Print();
                Console.Write("networkId: ");
                this.networkId.Print();
                Console.Write("hostId: ");
                this.hostId.Print();
                Console.Write("broadcast: ");
                this.broadcast.Print();
                Console.WriteLine();
            }
        }
        public static string GetNetworkId(string _ip, string _mask)
        {
            if (!Ip.CheckIp(_ip) || !Ip.CheckIp(_mask))
            {
                return "";
            }
            else
            {
                string[] ip = Ip.Create(_ip);
                string[] mask = Ip.Create(_mask);
                string networkId = "";
                for (int i = 0; i < 4; i++)
                {
                    networkId += Convert.ToString(Convert.ToInt32(ip[i]) & Convert.ToInt32(mask[i]));
                    networkId += i != 3 ? "." : "";
                }
                return networkId;
            }
        }
        public static string GetHostId(string _ip, string _mask)
        {
            if (!Ip.CheckIp(_ip) || !Ip.CheckIp(_mask))
            {
                return "";
            }
            else
            {
                string[] ip = Ip.Create(_ip);
                string[] networkId = Ip.Create(Network.GetNetworkId(_ip, _mask));
                string hostId = "";
                for (int i = 0; i < 4; i++)
                {
                    hostId += Convert.ToString(Convert.ToInt32(ip[i]) - Convert.ToInt32(networkId[i]));
                    hostId += i != 3 ? "." : "";
                }
                return hostId;
            }
        }
        // широковещательный 
        public static string GetBroadsast(string _ip, string _mask)
        {
            if (!Ip.CheckIp(_ip) || !Ip.CheckIp(_mask))
            {
                return "";
            }
            else
            {
                string[] mask = Ip.Create(_mask);
                string[] networkId = Ip.Create(Network.GetNetworkId(_ip, _mask));
                string broadcast = "";

                string[] newMask = new string[4];

                for (int i = 0; i < 4; i++)
                {
                    newMask[i] = Convert.ToString(Convert.ToInt32(mask[i]), 2);

                    if (newMask[i].Length < 8)
                    {
                        int l = newMask[i].Length;

                        for (int j = 0; j < 8 - l; j++)
                        {
                            newMask[i] = newMask[i].Insert(0, "0");
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < newMask[i].Length; j++)
                    {
                        if (newMask[i][j] == '1')
                        {
                            newMask[i] = newMask[i].Remove(j, 1);
                            newMask[i] = newMask[i].Insert(j, "0");

                            continue;
                        }
                        if (newMask[i][j] == '0')
                        {
                            newMask[i] = newMask[i].Remove(j, 1);
                            newMask[i] = newMask[i].Insert(j, "1");
                        }
                    }
                }

                int[] r = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    r[i] = Convert.ToInt32(newMask[i], 2);
                }

                for (int i = 0; i < 4; i++)
                {
                    broadcast += Convert.ToString(Convert.ToInt32(networkId[i]) | Convert.ToInt32(r[i]));

                    broadcast += i != 3 ? "." : "";
                }
                return broadcast;
            }
        }
    }
    class Program
    {
        public static void Main()
        {/*
            Console.WindowHeight = 13;
            Console.WindowWidth = 45;
            Console.ForegroundColor = ConsoleColor.Green;*/

            string ip = Convert.ToString(Console.ReadLine());


            string mask = Convert.ToString(Console.ReadLine());

            Network obj = new Network(ip, mask);
            obj.Print();

            Console.WriteLine("Console will be cleared!");

            Console.ReadKey();

            Console.Clear();

            Main();
        }
    }
}