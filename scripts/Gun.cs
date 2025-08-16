using Godot;
using System;

public partial class Gun : Sprite2D
{

    public override void _Process(double delta)
    {

        base._Process(delta);

        LookAt(GetGlobalMousePosition());
        switch (Math.Sign(GlobalPosition.DirectionTo(GetGlobalMousePosition())[0]))
        {
            case 1:
                this.FlipV = false;
                break;
            case -1:
                this.FlipV = true;
                break;
        }
    }
}
