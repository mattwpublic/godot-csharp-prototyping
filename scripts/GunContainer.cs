using Godot;
using System;

public partial class GunContainer : Node2D
{
    float fire_rate;
    float time_until_fire = 0f;
    [Export] float rps = 10;

    AudioStreamPlayer2D pistol_gunshots;
    Sprite2D gun;
    Muzzle muzzle;
    AnimationPlayer anim_player;

    public override void _Ready()
    {
        fire_rate = 1 / rps;
        pistol_gunshots = GetNode<AudioStreamPlayer2D>("Gun/Muzzle/PistolGunshotSFX");
        gun = GetNode<Sprite2D>("Gun");
        muzzle = GetNode<Muzzle>("Gun/Muzzle");
        anim_player = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {

        base._Process(delta);

        this.LookAt(GetGlobalMousePosition());
        switch (Math.Sign(GlobalPosition.DirectionTo(GetGlobalMousePosition()).X))
        {
            case 1:
                //gun.FlipV = false;
                break;
            case -1:
                //gun.FlipV = true;
                break;
        }

        if (Input.IsActionJustPressed("shoot"))
        {
            Shoot(delta);
        }
        else
        {
            time_until_fire += (float)delta;
        }
    }

    public void Shoot(double delta)
    {
        if (time_until_fire > fire_rate)
        {
            muzzle.FireBullet();
            time_until_fire = 0f;
            pistol_gunshots.Play();
            anim_player.Play("gun_shoot");
        }
    }
}
