using Godot;
using System;

public partial class Gun : Sprite2D
{
    float fire_rate;
    float time_until_fire = 0f;
    [Export] float rps = 10;

    AudioStreamPlayer2D pistol_gunshots;
    Muzzle muzzle;

    public override void _Ready()
    {
        fire_rate = 1 / rps;
        pistol_gunshots = GetNode<AudioStreamPlayer2D>("Muzzle/PistolGunshotSFX");
        muzzle = GetNode<Muzzle>("Muzzle");
    }

    public override void _Process(double delta)
    {

        base._Process(delta);

        LookAt(GetGlobalMousePosition());
        switch (Math.Sign(GlobalPosition.DirectionTo(GetGlobalMousePosition()).X))
        {
            case 1:
                this.FlipV = false;
                break;
            case -1:
                this.FlipV = true;
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
        }
    }
}
