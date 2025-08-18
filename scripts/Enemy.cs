using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] float GRAVITY = 2000.0f;
    [Export] float SPEED = 300f;
    [Export] float ACCELERATION = 1200f;
    Player player;
    Sprite2D body;
    Sprite2D gun;

    public override void _Ready()
    {
        base._Ready();
        player = null;
        body = GetNode<Sprite2D>("Body");
        gun = GetNode<Sprite2D>("Gun");
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var new_velocity = new Vector2(0, 0);

        //gravity
        if (!IsOnFloor())
        {
            new_velocity.Y += GRAVITY;
        }

        if (player != null)
        {
            gun.LookAt(player.GlobalPosition);
            var direction_to_player = player.GlobalPosition - this.GlobalPosition;
            if (Math.Sign(direction_to_player.X) < 0)
            {
                body.FlipH = true;
                gun.FlipV = true;
            }
            else
            {
                body.FlipH = false;
                gun.FlipV = false;
            }

            new_velocity.X = Mathf.MoveToward(Velocity.X, direction_to_player.Normalized().X * SPEED, (float)delta * ACCELERATION);
        }

        Velocity = new_velocity;
        MoveAndSlide();

    }

    public void _on_player_detector_body_entered(Node2D body)
    {
        GD.Print("body entered");
        if (body is Player)
        {
            if (player == null)
            {
                player = (Player)body;
                GD.Print("found player");
            }
        }
    }
}
