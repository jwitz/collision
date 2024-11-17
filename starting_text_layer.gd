extends CanvasLayer
var waitTime = 1
signal starting_sequence_completed
signal battery_statement_completed

var messages := [
	"ALERT: Power source has been compromised.",
	"Connection to mapping server has been severed.",
	"Last charge protocol is in effect.",
	"Clean as much as you can before you DIE."
]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Node2D/StartingText.visible = false

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass

func _on_menu_start_title_sequence() -> void:
	$Node2D/StartingText.visible = true
	for n in range(0,messages.size()):
		if n == 2:
			battery_statement_completed.emit()		
		$Node2D/StartingText.set_bbcode(messages[n])
		await get_tree().create_timer(4.0).timeout
		$Node2D/StartingText.fade_out = true
		await get_tree().create_timer(1.0).timeout
		$Node2D/StartingText.fade_out = false
	$Node2D/StartingText.visible = false
	starting_sequence_completed.emit()
		

	pass # Replace with function body.
