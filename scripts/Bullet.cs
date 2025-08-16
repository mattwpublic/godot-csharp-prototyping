using Godot;
using System;

public partial class Bullet : RigidBody2D
{

    Timer timer;
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.Timeout += () => QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (timer.TimeLeft < 0.4)
        {
            if (Math.Abs(this.LinearVelocity.X) < 0.5 || Math.Abs(this.LinearVelocity.Y) < 0.5)
            {
                QueueFree();
            }
        } 
    }


}
