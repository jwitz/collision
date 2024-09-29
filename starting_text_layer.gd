extends CanvasLayer

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Node2D/StartingText.set_bbcode("You have one charge left")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_menu_start_title_sequence() -> void:
	$Node2D/StartingText.set_bbcode("You have one charge left.")
	$Node2D/StartingText.set_bbcode("You have one charge left.")
	$Node2D/StartingText.set_bbcode("You have one charge left.")
	pass # Replace with function body.
