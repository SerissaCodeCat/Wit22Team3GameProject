using Godot;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

public partial class CharacterBody3d : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float GravityMultiplier = 1;
	[Export]
	public float MouseLookSensitivity = 0.01f;
	[Export]
	public float maxDegreesViewXRotation = 60.0f;
	[Export]
	public float minDegreesViewXRotation = -40.0f;
	private Vector3 DefaultGravity = new Vector3();
	private Vector3 CurrentGravity = new Vector3();
	private Node3D Head;
	private Node3D Camera;

    public override void _Ready()
    {
		DefaultGravity = new Vector3(0.0f, -(float)ProjectSettings.GetSetting("physics/3d/default_gravity"), 0.0f);
		GD.Print("Default gravity set to: " + DefaultGravity);
		CurrentGravity = DefaultGravity * GravityMultiplier;
		GD.Print("current gravity set to: " + CurrentGravity);
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Head = GetNode<Node3D>($"Head");
		Camera = GetNode<Camera3D>($"Head/PlayerCamera");
        base._Ready();
    }

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Access the relative movement (delta) since the last frame
            Vector2 moveView = mouseMotion.Relative * MouseLookSensitivity;
			GD.Print("Mouse Moved! : " + moveView);
			Head.RotateY(-moveView.X);
			Camera.RotateX(-moveView.Y);
			Vector3 cameraRotation = Camera.Rotation;

    		float minX = Mathf.DegToRad(minDegreesViewXRotation);
    		float maxX = Mathf.DegToRad(maxDegreesViewXRotation);
    		if (cameraRotation.X <= minX)
			{
				cameraRotation.X = minX;
				Camera.Rotation = cameraRotation;
			}
			else if (cameraRotation.X >= maxX)
			{
				cameraRotation.X = maxX;
				Camera.Rotation = cameraRotation;
			}
            
            // You can also get the current global position
            //Vector2 currentPosition = mouseMotion.Position;

            // Use the delta for movement logic, e.g., camera rotation
            // GD.Print($"Mouse moved by: {delta}");
        }
    }
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
		{
			GD.Print("In the air, movement set to: " + CurrentGravity);
			velocity += CurrentGravity * (float)delta;
		}
		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") /*&& IsOnFloor()*/)
		{
			GD.Print("Jump Pressed");
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("StrafeLeft", "StrafeRight", "Forwards", "Backwards");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
