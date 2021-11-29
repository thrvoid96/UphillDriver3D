using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using UnityEngine;

public class PlayerController : CommonBehaviours
{
    public static PlayerController instance;
    
    private bool var1, var2, var3;
    private float distance,moveDuration;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Update()
    {
        base.Update();
        Movement();
    }

    protected override void Start()
    {
        base.Start();
            
        StartCoroutine(nameof(Movement));
    }
    
    private void Movement()
    {
        if (LevelManager.gameState == GameState.Normal && canMove) 
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            if (!isOnRamp)
            { 
                var1 = false; 
                var2 = false; 
                var3 = false;
                
                transform.Translate(maxSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self); 
                transform.Rotate(0, horizontalInput * verticalInput * 2f * Time.deltaTime * 150f, 0, Space.Self);
                
            }
            else
            
            { 
                if (verticalInput > 0) 
                { 
                    distance = Vector3.Distance(transform.position, finalPos); 
                    moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f); 
                    
                    if (!var1) 
                    { 
                        transform.DOKill();
                        
                        transform.DOMove(finalPos,moveDuration * speedRatio).OnComplete(() => 
                        { 
                            if (coefficient == 0.9f) 
                            { 
                                ExitRampComplete(); 
                                Debug.LogWarning("Üst rampaya çıktın"); 
                            }
                            
                        });
                        
                        StartTrails();
                        
                        var1 = true; 
                        var2 = false; 
                        var3 = false; 
                    } 
                }
                else if (verticalInput == 0)
                
                {
                    distance = Vector3.Distance(transform.position, rampStartPos); 
                    moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);     
                    
                    if(rampStartPos.y < transform.position.y)
                    { 
                        if (!var2) 
                        { 
                            transform.DOKill();
                            
                            transform.DOMove(rampStartPos, moveDuration * 4f * speedRatio).SetEase(Ease.InExpo);
                            
                            StartTrails();
                            
                            var1 = false; 
                            var2 = true; 
                            var3 = false; 
                        }
                        
                    }

                }
                else
                { 
                    distance = Vector3.Distance(transform.position, rampStartPos); 
                    moveDuration = Mathf.Clamp(distance / 20f, 0f, 2f); 
                    if (!var3)
                    { 
                        transform.DOKill();
                        
                        transform.DOMove(rampStartPos, moveDuration * 1.5f * speedRatio).SetEase(Ease.InOutQuint)
                            .OnComplete(() => 
                            { 
                                ExitRamp(); 
                                Debug.LogWarning("Rampadan indin"); 
                            }); 
                        
                        StartTrails();
                        
                        var1 = false; 
                        var2 = false; 
                        var3 = true; 
                    } 
                } 
            } 
        } 
    }
}
