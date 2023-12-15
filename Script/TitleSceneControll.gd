extends Button


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	
	pass # Replace with function body.
func _end_game():
	get_tree().quit();
func _start_game():
	get_tree().change_scene_to_file("res://Scenes/new scene.tscn");
func _setting():
	print("setting");
