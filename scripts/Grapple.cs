using Godot;
using System;

public partial class Grapple : Node2D
{
    [Export] public float GRAPPLE_STRENGTH = 15f;
    [Export] public float GRAPPLE_STRENGTH_DOWN = 0.3f;
    [Export] public float GRAPPLE_STRENGTH_UP = 1.1f;
    [Export] public float GRAPPLE_STRENGTH_OPPOSITE = 0.75f;
    [Export] public float GRAPPLE_DAMPEN = 0.95f;
    [Export] public float GRAPPLE_MAX_RANGE = 200f;
    [Export] public float GRAPPLE_MAX_LENGTH = 210f;

    private Vector2 grapple_velocity = new Vector2(0, 0);
    public Vector2 grapple_direction = new Vector2(0, 0);
    public Vector2 grapple_tip_vector = new Vector2(0, 0);

    public const float GRAPPLE_TIP_SPEED = 15;

    public bool flying = false;
    public bool hooked = false;

    Line2D grapple_line;
    public CharacterBody2D grapple_bullet;
    AudioStreamPlayer2D grapple_shoot_sfx;
    AudioStreamPlayer2D grapple_hit_sfx;

    public override void _Ready()
    {
        base._Ready();
        grapple_line = GetNode<Line2D>("GrappleLine");
        grapple_bullet = GetNode<CharacterBody2D>("GrappleBullet");
        grapple_shoot_sfx = GetNode<AudioStreamPlayer2D>("GrappleShootSFX");
        grapple_hit_sfx = GetNode<AudioStreamPlayer2D>("GrappleHitSFX");
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        this.Visible = flying || hooked;
        if (!this.Visible)
        {
            return;
        }
        var tip_location = ToLocal(grapple_tip_vector);
        grapple_line.SetPointPosition(1, tip_location);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        grapple_bullet.GlobalPosition = grapple_tip_vector;

        if (flying)
        {
            if ((grapple_tip_vector - this.GlobalPosition).Length() > GRAPPLE_MAX_RANGE)
            {
                Release();
                return;
            }
            var collision_data = grapple_bullet.MoveAndCollide(grapple_direction.Normalized() * GRAPPLE_TIP_SPEED);
            if (collision_data != null)
            {
                hooked = true;
                flying = false;
                grapple_hit_sfx.Play();
            }
        }
        grapple_tip_vector = grapple_bullet.GlobalPosition;

    }

    public void Shoot()
    {
        grapple_direction = GetGlobalMousePosition() - this.GlobalPosition;
        flying = true;
        grapple_tip_vector = this.GlobalPosition;
        grapple_shoot_sfx.Play();
    }

    public void Release()
    {
        grapple_tip_vector = this.GlobalPosition;
        grapple_bullet.GlobalPosition = this.GlobalPosition;
        flying = false;
        hooked = false;
    }

}
