using System;

public class User
{
    public string id;
    public string username;
    public int score;
    public string avatarImage;

    private static readonly Random random = new Random();

    public User(string id, string username, int score)
    {
        this.id = id;
        this.username = username;
        this.score = score;
        setAvatarImage();
    }

    private void setAvatarImage()
    {
        avatarImage = "car_" + random.Next(1, 8); ;
    }
}
