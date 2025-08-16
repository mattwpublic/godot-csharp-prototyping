using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] float SPEED = 350.0f;
    [Export] float ACCELERATION = 1200.0f;
    [Export] float FRICTION = 1400.0f;
    [Export] float FLOOR_DAMPING = 1.0f;

    [Export] float GRAVITY = 2000.0f;
    [Export] float FALL_GRAVITY = 5000.0f;
    [Export] float FAST_FALL_GRAVITY = 6000.0f;
    [Export] float WALL_GRAVITY = 25.0f;

    [Export] float JUMP_VELOCITY = -700.0f;
    [Export] float WALL_JUMP_VELOCITY = -700.0f;
    [Export] float WALL_JUMP_PUSHBACK = -300.0f;

    [Export] float INPUT_BUFFER_PAITIENCE = 0.1f;
    [Export] float COYOTE_TIME = 0.08f;

    Timer input_buffer;
    Timer coyote_timer;

    private Vector2 _velocity;
    private Vector2 input_vector;
    private Vector2 grapple_velocity = new Vector2(0, 0);

    bool jump_attempted = false;
    bool coyote_jump_available = true;
    bool air_jump_available = true;

    Sprite2D body_sprite;
    Grapple grapple;
    AudioStreamPlayer2D jump_sfx;
    AudioStreamPlayer2D air_jump_sfx;

    public override void _Ready()
    {
        base._Ready();

        input_buffer = new Timer();
        input_buffer.WaitTime = INPUT_BUFFER_PAITIENCE;
        input_buffer.OneShot = true;
        AddChild(input_buffer);

        coyote_timer = new Timer();
        coyote_timer.WaitTime = COYOTE_TIME;
        coyote_timer.OneShot = true;
        AddChild(coyote_timer);
        coyote_timer.Timeout += CoyoteTimeout;

        body_sprite = GetNode<Sprite2D>("Body");
        grapple = GetNode<Grapple>("Grapple");
        jump_sfx = GetNode<AudioStreamPlayer2D>("JumpSFX");
        air_jump_sfx = GetNode<AudioStreamPlayer2D>("AirJumpSFX");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton)
        {
            if (@event.IsActionPressed("grapple"))
            {
                grapple.Shoot();
            }
            else
            {
                grapple.Release();
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        switch (Math.Sign(GlobalPosition.DirectionTo(GetGlobalMousePosition()).X))
        {
            case 1:
                body_sprite.FlipH = false;
                break;
            case -1:
                body_sprite.FlipH = true;
                break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        //This doesn't do anything right now. When you implement sword slashes, this might be helpful though.
        float dash_multiplier = 1;

        input_vector = GetInputVector();

        //Gravity
        if(IsOnFloor())
        {
            coyote_jump_available = true;
            air_jump_available = true;
            coyote_timer.Stop();
        }
        else {
            if(coyote_jump_available && coyote_timer.IsStopped())
            {
                coyote_timer.Start();
            }
            PlusEqualsVelocityY(CalculateGravity(input_vector) * (float)delta);
            
        }

        //jumping
        jump_attempted = Input.IsActionJustPressed("jump");
        if(jump_attempted || input_buffer.TimeLeft > 0)
        {
            if (jump_attempted)
            {
                GD.Print("jump attempted");
            }
            else if (input_buffer.TimeLeft > 0)
            {
                GD.Print("no jump attempted, but input buffer had time left.");
            }
            if (coyote_jump_available)
            {
                GD.Print("coyote jump was available");
                EqualsVelocityY(JUMP_VELOCITY);
                coyote_jump_available = false;
                jump_sfx.Play();
            }
            else if (air_jump_available && !IsOnWall())
            {
                GD.Print("air jump was available");
                EqualsVelocityY(JUMP_VELOCITY);
                air_jump_available = false;
                air_jump_sfx.Play();
            }
            else if (IsOnWall() && input_vector.X != 0)
            {
                EqualsVelocityY(WALL_JUMP_VELOCITY);
                EqualsVelocityX(WALL_JUMP_PUSHBACK * Math.Sign(input_vector.X));
                Velocity = _velocity;
                jump_sfx.Play();
            }
            else if (jump_attempted)
            {
                GD.Print("Jump attempted, but no conditions were met so the buffer was started.");
                input_buffer.Start();
            }
        }

        FLOOR_DAMPING  = IsOnFloor() ? 1.0f : 0.2f;

        //Moving left and right
        if(input_vector.X != 0)
        {
            EqualsVelocityX(Mathf.MoveToward(Velocity.X, input_vector.X * SPEED * dash_multiplier, ACCELERATION * (float)delta));
        }
        else if (IsOnFloor() && grapple_velocity.X == 0)
        {
            EqualsVelocityX(Mathf.MoveToward(Velocity.X, 0, FRICTION * (float)delta) * FLOOR_DAMPING);
        }

        //Grapple
        if (grapple.hooked)
        {
            //calculate initial grapple velocity based on vector magnitude
            grapple_velocity = (grapple.grapple_tip_vector - this.GlobalPosition).Normalized() * grapple.GRAPPLE_STRENGTH;

            //if the player reaches the max length of the grapple rope, it should not let them go any further
            if ((grapple.grapple_tip_vector - this.GlobalPosition).Length() >= grapple.GRAPPLE_MAX_LENGTH)
            {
                //set the players position to the furthest possible position such that the max length is correct
                var new_player_position = ((this.GlobalPosition - grapple.grapple_tip_vector).Normalized() * grapple.GRAPPLE_MAX_LENGTH) + grapple.grapple_bullet.GlobalPosition;
                this.GlobalPosition = new_player_position;
                
                //this.Velocity = new Vector2(0, 0);
                EqualsVelocityY(0);
            }

            //try to add more power to grapple as it stretches to its maximum range
            if ((grapple.grapple_tip_vector - this.GlobalPosition).Length() >= grapple.GRAPPLE_MAX_RANGE)
            {
                grapple_velocity *= 1.01f;
            }

            //the grapple is weaker when going down, and stronger when going up
            if (grapple_velocity.Y > 0)
            {
                grapple_velocity.Y *= grapple.GRAPPLE_STRENGTH_DOWN;
            }
            else
            {
                grapple_velocity.Y *= grapple.GRAPPLE_STRENGTH_UP;
            }

            //if the player is moving against the grapple's pull, it will weaken, otherwise it dampens everything
            if ((grapple_velocity.X < 0 && input_vector.X < 0) || (grapple_velocity.X > 0 && input_vector.X > 0))
            {
                grapple_velocity.X *= grapple.GRAPPLE_DAMPEN;
            }
            else
            {
                grapple_velocity.X *= grapple.GRAPPLE_STRENGTH_OPPOSITE;
            } 
        }
        else
        {
            grapple_velocity = new Vector2(0, 0);
        }
        this.Velocity += grapple_velocity;

        MoveAndSlide();
    }

    private Vector2 GetInputVector()
    {
        return Input.GetVector("move_left", "move_right", "move_up", "move_down");
    }

    private void CoyoteTimeout()
    {
        coyote_jump_available = false;
    }

    private float CalculateGravity(Vector2 _input_direction)
    {
        if(Input.IsActionPressed("move_down"))
        {
            return FAST_FALL_GRAVITY;
        }
        if(IsOnWallOnly() && Velocity.Y > 0 && _input_direction.X != 0)
        {
            return WALL_GRAVITY;
        }
        if (Velocity.Y < 0 || grapple.hooked)
        {
            return GRAVITY;
        }
        else
        {
            return FALL_GRAVITY;
        }
    }

    private void EqualsVelocityX(float delta_velocity_x)
    {
        _velocity = Velocity;
        _velocity.X = delta_velocity_x;
        Velocity = _velocity;
    }

    private void EqualsVelocityY(float delta_velocity_y)
    {
        _velocity = Velocity;
        _velocity.Y = delta_velocity_y;
        Velocity = _velocity;
    }

    private void PlusEqualsVelocityY(float delta_velocity_y)
    {
        _velocity = Velocity;
        _velocity.Y += delta_velocity_y;
        Velocity = _velocity;
    }
}
