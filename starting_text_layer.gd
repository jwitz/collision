extends CanvasLayer
var waitTime = 1

var messages := [
	"You have one charge left.",
	"Clean your house as best you can.",
]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Node2D/StartingText.visible = false

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_menu_start_title_sequence() -> void:
	$Node2D/StartingText.visible = true
	for n in range(0,messages.size()):
		$Node2D/StartingText.set_bbcode(messages[n])
		await get_tree().create_timer(5.0).timeout
		$Node2D/StartingText.fade_out = true
		await get_tree().create_timer(2.0).timeout
		$Node2D/StartingText.fade_out = false

	pass # Replace with function body.
