extends CharacterBody2D

var inputDirectionY: float
var inputDirectionX: float
var direction: Vector2

var speed: float = 512
var sprint: float = 1

var origin: Vector2
var collision_point: Vector2
var distance: float

var index: int

var theme = preload("res://theme.tres")

@export var raycastDivider: float = 16
@export var fov: float = 90

var camera: Camera2D

var panel: Panel
@onready var panels: Control = $Camera2D/CanvasLayer/Panels

func _input(event):
	if event is InputEventMouseMotion:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		rotation += event.relative.x * 0.001


	
		
func _ready():
	var raycastAmount: float = 1920.0/raycastDivider
	for i in raycastAmount:
		var raycast = RayCast2D.new()
		raycast.name = str(i)
		raycast.target_position.y = 10000
		raycast.rotation = deg_to_rad(-fov/2 + ((float(i)+0.5) * (fov/raycastAmount)))
		$raycasts.add_child(raycast)
		
		panel = Panel.new()
		panel.name = str(i)
		panel.size = Vector2(1920/raycastAmount, 1)
		panel.position.x += i * 1920/raycastAmount
		panels.add_child(panel)
		
func _process(delta: float) -> void:
	inputDirectionY = Input.get_axis("up", "down")
	inputDirectionX = Input.get_axis("left", "right")
	direction = Vector2(-inputDirectionX, -inputDirectionY).rotated(rotation).normalized()
	velocity = velocity.lerp(direction * speed, 5 * delta)
	move_and_slide()

	

	for i in $raycasts.get_children():
		index = i.get_index()
		panel = panels.get_child(index)
		
		if i.is_colliding():
			collision_point = i.get_collision_point()
			distance = global_transform.origin.distance_to(collision_point)
			distance = distance * cos(i.rotation)
			panel.size.y = lerp(0, 2048, 100/distance)
			panel.position.y = -panel.size.y/2
		else:
			panel.size.y = 0
