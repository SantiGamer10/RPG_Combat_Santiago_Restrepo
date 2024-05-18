# RPG_Combat_Santiago_Restrepo

Hecho con Unity 2022.3.12f1

El código fue hecho para encontrar y quitar code smells y refactorear usando las reglas SO de SOLID y Clean Code.

Cosas que se arreglaron:

Clase GameController:

- Se repetía mucho código en el método Update, por lo que se dejaron solo los if, y se hizo un método que se llame MoveToLeft, MoveToRight, etc, etc en cada caso.

- En cada método, se utilizó uno llamado CheckIfIsValidPosition() dentro de los if. Este método tiene un && entre 2 métodos internamente: CheckIfCellExists, CheckIfCellIsNotATree.

- Se hizo un método MoveCharacterToPosition() que realice el movimiento del jugador y chequee si ganó.

- El método InitializeMap se puede llamar al final con MoveCharacterToPosition().

Clase InitializeMap:

- Se pusieron las configuraciones (public static) en una clase que se llame configs.cs.

- Se puso el enum TerrainType en un script llamdo TerrainType.cs.

Cosas adicionales:

- Separación de clases en vista y comportamiento: Solo se dejó lo que está en Start, Update y el mostrar si se gsnó en el MonoBehavior, y todo lño demás se puso en una clase que no herede de este.
