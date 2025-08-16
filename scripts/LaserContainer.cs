using Godot;
using System;

public partial class LaserContainer : Node2D
{
    public override void _Process(double delta) {

        base._Process(delta);

        var laser_line = (Line2D)GetNode("LaserLine2D");
        var laser_ray_cast = (RayCast2D)GetNode("LaserRayCast");
        var laser_point = (Sprite2D)GetNode("LaserPoint");
        var laser_point_ray_cast = (RayCast2D)GetNode("LaserPointRayCast");

        if(laser_ray_cast.IsColliding()) {
            var laser_line_collision_point = laser_ray_cast.GetCollisionPoint();
            var local_laser_line_collision_point = ToLocal(laser_line_collision_point);
            
            laser_line.SetPointPosition(1, local_laser_line_collision_point);
        }
        else {
            laser_line.SetPointPosition(1, new Vector2(100, 0));
        }

        if(laser_point_ray_cast.IsColliding()) {
            var point = laser_point_ray_cast.GetCollisionPoint();
            var local_point = ToLocal(point);
            
            laser_point.Visible = true;
            laser_point.SetPosition(local_point);
        }
        else {
            laser_point.Visible = false;
        }
    }
}
