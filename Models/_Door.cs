namespace grimtol_checkpoint.Models
{
  public class Door
  {
    public string Name { get; set; }
    public bool Locked { get; set; }

    public Door()
    {
      Name = "Room Door";
      Locked = false;
    }
  }
}