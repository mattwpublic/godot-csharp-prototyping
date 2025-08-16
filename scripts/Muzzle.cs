using Godot;
using System;

public partial class Muzzle : Node2D
{

    [Export] PackedScene bulletScene;
    [Export] float bullet_speed = 1000f;
    [Export] float bullet_damage = 30f;

    public void FireBullet()
    {
        RigidBody2D bullet = bulletScene.Instantiate<RigidBody2D>();

        bullet.Rotation = GlobalRotation;
        bullet.GlobalPosition = GlobalPosition;
        bullet.LinearVelocity = bullet.Transform.X * bullet_speed;

        GetTree().Root.AddChild(bullet);
    }

}
