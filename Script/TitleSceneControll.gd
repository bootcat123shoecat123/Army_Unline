extends Button


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
@onready var animation_Texture:TextureRect=$"../../AnimationPlayer/TextureRect"
@onready var animation_Player:AnimationPlayer=$"../../AnimationPlayer"

# Called when the node enters the scene tree for the first time.
func _ready():
	
	pass # Replace with function body.
func _end_game():
	get_tree().quit();
func _start_game():
	animation_Texture.visible=true
	animation_Player.queue("fade_in")
	
	
func _setting():
	print("setting");


func _on_animation_player_animation_finished(anim_name):
	if(anim_name=="fade_in"):
		get_tree().change_scene_to_file("res://Scenes/GameLevel/Level001.tscn");
