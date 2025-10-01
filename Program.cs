namespace DigitalWallet
{
    internal class Program
    {
        static void Main()
        {
            DataSet ds = new DataSet();
            Menu menu = new Menu(ds);
            menu.Run();
        }
    }
}