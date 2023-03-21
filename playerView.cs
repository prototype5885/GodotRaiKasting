using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Godot.Projection;

public partial class playerView : Node2D
{
	float inputDirectionY;
	float inputDirectionX;
	Vector2 inputDirection;

	float speed = 512;

	Vector2 origin;
	Vector2 collisionPoint;
	float distance;

	int index;

	public int raycastDivider = 1;

    Camera2D camera;
    Panel panel;

    Vector2 localTargetPosition;
    float localRotation;
    Vector2 localPosition;
    Vector2 localPosition2;
	Vector2 localSize;
	Vector2 localPosition3;
	float localRotation2;

	Node2D raycasts;
	Control panels;


    public override void _Input(InputEvent @event)
    {
       if (@event is InputEventMouseMotion eventMouseMotion)
		{
            Input.MouseMode = Input.MouseModeEnum.Captured;
            localRotation2 = Rotation;
			localRotation2 = localRotation2 + eventMouseMotion.Relative.X * 0.001f;
			Rotation = localRotation2;

        }
            
    }

    public override void _Ready()
	{
		panels = GetNode<Control>("/root/Node2D/eye/Camera2D/CanvasLayer/Panels");
        raycasts = GetNode<Node2D>("/root/Node2D/eye/raycasts");

        float raycastAmount = 1920.0f / raycastDivider;


		for (int i = 0; i < raycastAmount; i++)
		{
			RayCast2D raycast = new RayCast2D();
			raycast.Name = i.ToString();
			localTargetPosition = raycast.TargetPosition;
			localTargetPosition.Y = 10000;
			raycast.TargetPosition = localTargetPosition;

			localRotation = raycast.Rotation;
			localRotation = Mathf.DegToRad(-45f + ((i + 0.5f) * (90.0f / raycastAmount)));
			GetNode<Node2D>("/root/Node2D/eye/raycasts").AddChild(raycast);


			Panel panel = new Panel();
			panel.Name = i.ToString();
			panel.Size = new Vector2(1920 / raycastAmount, 1);
			localPosition = panel.Position;
			localPosition.X = localPosition.X + i * 1920 / raycastAmount;
			panel.Position = localPosition;
			panels.AddChild(panel);
		}

    }
	public override void _Process(double delta)
	{
		inputDirectionY = Input.GetAxis("up", "down");
		inputDirectionX = Input.GetAxis("left", "right");
		inputDirection = new Vector2(inputDirectionX, inputDirectionY);
		//localPosition2 = Position;
		//localPosition2.X = localPosition2.X + inputDirectionX * speed;
		//localPosition2.Y = localPosition2.Y + inputDirectionY * speed;
		//Position = localPosition2;

		localPosition2 = localPosition2 - inputDirection.Rotated(Rotation).Normalized() * speed * Convert.ToSingle(delta);
		Position = localPosition2;

		foreach (RayCast2D i in raycasts.GetChildren())
		{
			if(i.IsColliding())
			{
				index = i.GetIndex();
				panel = (Panel)panels.GetChild(index);

				origin = GlobalTransform.Origin;
				collisionPoint = i.GetCollisionPoint();
				distance = origin.DistanceTo(collisionPoint);
				localSize = panel.Size;
				localSize.Y = Mathf.Lerp(0, 4096, 100/distance);
				panel.Size = localSize;
				localPosition3 = panel.Position;
				localPosition3.Y = -panel.Size.Y / 2;
				panel.Position = localPosition3;
            }
			else
			{
                panel.Size = localSize;
				localPosition3.Y = 0;
                panel.Position = localPosition3;
            }
        }
    }
}
