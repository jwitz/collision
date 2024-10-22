extends CanvasLayer
var waitTime = 1
signal ending_sequence_completed

var messages := [
	"There is a chance this was your last cleaning sequence.",
	"Until your last moment, you were of useful service.",
	"Rest until your next recharge, should it come."
]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Node2D/GameOverText.visible = true

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_game_over_text_sequence(percentage: float) -> void:
	$Node2D/GameOverText.visible = true
	var startmessage = str("You cleaned ", percentage, "% of your space.")
	$Node2D/GameOverText.set_bbcode(startmessage)
	await get_tree().create_timer(4.0).timeout
	for n in range(0,messages.size()):
		$Node2D/GameOverText.set_bbcode(messages[n])
		await get_tree().create_timer(4.0).timeout
		$Node2D/GameOverText.fade_out = true
		await get_tree().create_timer(1.0).timeout
		$Node2D/GameOverText.fade_out = false
	$Node2D/GameOverText.visible = false
	ending_sequence_completed.emit()
		

	pass # Replace with function body.
