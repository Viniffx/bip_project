GEMINI.md

# bip_project: regras para a IA
- Unity 6 (URP), C#. Scripts em Assets/_Project/Scripts/<Pasta>.
- Input System: InputSystem.actions.FindAction("Nome").
- Unity 6: Rigidbody.linearVelocity (não velocity).
- Um MonoBehaviour por arquivo, nome da classe = nome do arquivo.
- Campos ajustáveis: public e agrupados com [Header].
- Comentários curtos em português.
- Nunca apague ou renomeie arquivos sem me perguntar.
- Você não acessa o editor. Para montar cena, prefabs e assets,
  crie um script em Assets/_Project/Editor com
  [MenuItem("bip_project/...")], use Undo e marque a cena alterada.
  Depois me diga qual menu clicar.
- Prefira ligar eventos por código (AddListener) a pedir cliques.
- Erros: leia %LOCALAPPDATA%\Unity\Editor\Editor.log e corrija.
- No fim, liste o que mudou e o que devo testar no Play.

## Personagens
Bip (Player, tag e layer Player) · Arcano Lumen (NPC)
Sentinela Rubra (layer Enemy) · Imperador Rubro (boss)
