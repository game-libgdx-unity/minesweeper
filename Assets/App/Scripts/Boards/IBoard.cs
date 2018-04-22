namespace App.Scripts.Boards
{
    public interface IBoard
    {
        int Width { get; set; }
        int Height { get; set; }
        int MineCount { get; set; }
    }
}