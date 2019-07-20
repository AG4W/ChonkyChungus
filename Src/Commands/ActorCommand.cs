public abstract class ActorCommand : Command
{
    public Actor actor { get; private set; }

    /// <summary>
    /// ActorCommands adds themselves to the specified actors command queue,
    /// Calling ".Execute()" outside of the queue will lead to double executions of the same command.
    /// </summary>
    /// <param name="actor"></param>
    public ActorCommand(Actor actor)
    {
        this.actor = actor;
        this.actor.AddCommand(this);
    }
}