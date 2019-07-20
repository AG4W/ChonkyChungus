public class Mission
{
    public string title { get; private set; }

    public Task[] tasks { get; private set; }

    public Mission(string title, params Task[] tasks)
    {
        this.title = title;
        this.tasks = tasks;
    }

    public override string ToString()
    {
        string s = "";

        for (int i = 0; i < tasks.Length; i++)
            s += tasks[i].ToString() + "\n\n";

        return s;
    }
}