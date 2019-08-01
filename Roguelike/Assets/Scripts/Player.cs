using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int pointsPerFood = 10;                //Number of points to add to player food points when picking up a food object.
    public int pointsPerSoda = 20;                //Number of points to add to player food points when picking up a soda object.
    public int wallDamage = 1;                 //How much damage a player does to a wall when chopping it.
    public Text foodText;

    private Animator animator;                    //Used to store a reference to the Player's animator component.
    private int food;                            //Used to store player food points total during level.

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;

        base.Start();
    }


    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }
    private void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;      //Used to store the horizontal move direction.
        int vertical = 0;        //Used to store the vertical move direction.


        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }
     protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);

         RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1,moveSound2);

        }
         
         CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }


     protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
         hitWall.DamageWall(wallDamage);

        animator.SetTrigger("playerChop");
    }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
             Invoke("Restart", restartLevelDelay);

             enabled = false;
        }

        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood+"Food: "+ food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }

         else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + "Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }


    private void Restart()
    {
        SceneManager.LoadScene(0);
    }


    public void LoseFood(int loss)
    {
         animator.SetTrigger("playerHit");

        food -= loss;
        foodText.text = "-" + loss + "Food: " + food;
        CheckIfGameOver();
    }

     private void CheckIfGameOver()
    {
         if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
