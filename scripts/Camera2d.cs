using Godot;
using System;

public partial class Camera2d : Camera2D
{
    int DEAD_ZONE = 100;
    float MAX_FOV = 200f;
    float CLOSE_SMOOTHING = 32.0f;
    float FAR_SMOOTHING = 50.0f;
    float ZOOM_AMOUNT = 1f;
    float MAX_ZOOM_OUT = -200f;
    float MAX_ZOOM_IN = 10f;
    
    Vector2 OFFSET = new Vector2(60.0f, 0.0f);

    AnimationPlayer camera_animator;
    AudioStreamPlayer2D camera_flip_sfx;
    AudioStreamPlayer music;
    
    bool flip_flag_old = true;
    bool flip_flag_new = true;
    Vector2 new_zoom;

    public override void _Ready()
    {
        camera_animator = (AnimationPlayer)GetNode("CameraAnimator");
        camera_flip_sfx = (AudioStreamPlayer2D)GetNode("CameraFlipSFX");
        music = GetNode<AudioStreamPlayer>("Music");
        music.Play();
    }

    public override void _Process(double delta)
    {

        base._Process(delta);

        if (GetGlobalMousePosition().X - this.GlobalPosition.X > DEAD_ZONE)
        {
            flip_flag_new = true;
            if (flip_flag_new != flip_flag_old)
            {
                camera_animator.Play("flip_left_to_right");
                flip_flag_old = true;
                camera_flip_sfx.Play();
            }
        }
        else if (GetGlobalMousePosition().X - this.GlobalPosition.X < -DEAD_ZONE)
        {
            flip_flag_new = false;
            if (flip_flag_new != flip_flag_old)
            {
                camera_animator.Play("flip_right_to_left");
                flip_flag_old = false;
                camera_flip_sfx.Play();
            }
        }

        Node2D player = (Node2D)GetParent();
        Vector2 target_vector = GetGlobalMousePosition() - player.GlobalPosition;
        if (target_vector.Length() < DEAD_ZONE)
        {
            this.PositionSmoothingSpeed = CLOSE_SMOOTHING;
            this.Position = new Vector2(0, 0);
        }
        else
        {
            this.PositionSmoothingSpeed = FAR_SMOOTHING;
            this.Position = target_vector.Normalized() * Mathf.Clamp((target_vector.Length() - DEAD_ZONE), 0, MAX_FOV) * 0.5f;
        }

        if (Input.IsActionJustPressed("camera_zoom_in"))
        {
            new_zoom.X = this.Zoom.X + ZOOM_AMOUNT;
            new_zoom.Y = this.Zoom.Y + ZOOM_AMOUNT;
            if (new_zoom.X > MAX_ZOOM_IN)
            {
                new_zoom.X = MAX_ZOOM_IN;
                new_zoom.Y = MAX_ZOOM_IN;
            }
            this.Zoom = new_zoom;
        }
        if (Input.IsActionJustPressed("camera_zoom_out"))
        {
            new_zoom.X = this.Zoom.X - ZOOM_AMOUNT;
            new_zoom.Y = this.Zoom.Y - ZOOM_AMOUNT;
            if (new_zoom.X < MAX_ZOOM_OUT)
            {
                new_zoom.X = MAX_ZOOM_OUT;
                new_zoom.Y = MAX_ZOOM_OUT;
            }
            this.Zoom = new_zoom;
        }
    }
}
