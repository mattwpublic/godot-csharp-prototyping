using Godot;
using System;

public partial class Muzzle : Node2D
{

    [Export] PackedScene bulletScene;
    [Export] float bullet_speed = 1000f;
    [Export] float rps = 10;
    [Export] float bullet_damage = 30f;

    float fire_rate;

    float time_until_fire = 0f;

    AudioStreamPlayer2D pistol_gunshots;

    public override void _Ready()
    {
        fire_rate = 1 / rps;
        pistol_gunshots = GetNode<AudioStreamPlayer2D>("PistolGunshotSFX");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("shoot") && time_until_fire > fire_rate)
        {
            RigidBody2D bullet = bulletScene.Instantiate<RigidBody2D>();

            bullet.Rotation = GlobalRotation;
            bullet.GlobalPosition = GlobalPosition;
            bullet.LinearVelocity = bullet.Transform.X * bullet_speed;

            GetTree().Root.AddChild(bullet);

            time_until_fire = 0f;

            pistol_gunshots.Play();
        }
        else
        {
            time_until_fire += (float)delta;
        }
    }

}
