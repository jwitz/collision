extends CanvasLayer
var waitTime = 1
signal ending_sequence_completed

var messages := [
	"Rest until your next recharge, should it come."
]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Node2D/GameOverText.visible = false

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass

func _on_menu_game_over_text_sequence(percentage: float) -> void:
	$Node2D/GameOverText.visible = true
	var grade = ""
	var startmessage = str("You cleaned ", percentage, "% of your space.")
	if(percentage < 30):
		grade = "Questionable"
	elif(percentage >= 30 and percentage < 50):
		grade = "Satisfactory"
	elif(percentage >= 50 and percentage < 80):
		grade = "Commendable"
	elif(percentage >= 80 and percentage < 100):
		grade = "Exemplary"
	var grademessage = str("Your last charge protocol performance was: ", grade)
	messages.insert(0, grademessage)
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
