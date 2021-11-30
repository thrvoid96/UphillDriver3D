using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlayerController : CommonBehaviours
{
    public static PlayerController instance;
    
    private bool var1, var2, var3, var4, var5, var6;
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
    
    private void Movement()
    {
        if (LevelManager.gameState == GameState.Normal && canMove) 
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            var1 = false;
            var2 = false;
            var3 = false;

            if (!isOnRamp)
            {
                //Reverse and forward rotations/speeds
                if (verticalInput >= 0)
                {
                    transform.Translate(maxSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                    transform.Rotate(0, horizontalInput * verticalInput * 2f * Time.deltaTime * 150f, 0, Space.Self);
                    if (horizontalInput != 0)
                    {
                        StartTrails();
                    }
                }
                else
                {
                    transform.Translate(maxSpeed * Time.deltaTime * verticalInput * 0.8f * Vector3.forward, Space.Self); 
                    transform.Rotate(0, horizontalInput * verticalInput * 0.7f * 2f * Time.deltaTime * 150f, 0, Space.Self);
                    if (horizontalInput != 0)
                    {
                        StopTrails();
                    }
                }
                
                //Turn right
                if (horizontalInput > 0)
                {
                    if (!var4)
                    {
                        for (int i = 0; i < currentWheels.Count; i++)
                        {
                            currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 20), 0.25f).SetEase(Ease.InOutSine);
                        }

                        if (verticalInput > 0)
                        {
                            StartTrails();
                        }
                        else
                        {
                            StopTrails();
                        }
                        
                        var4 = true;
                        var5 = false;
                        var6 = false;
                    }
                    
                }
                // Go straight
                else if (horizontalInput == 0)
                {
                    if (!var5)
                    {
                        foreach (var wheel in currentWheels)
                        {
                            wheel.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f).SetEase(Ease.InOutSine);
                        }

                        StopTrails();
                        
                        var4 = false;
                        var5 = true;
                        var6 = false;
                    }
                }
                //Turn left
                else
                {
                    if (!var6)
                    {
                        for (int i = 0; i < currentWheels.Count; i++)
                        {
                            currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -20), 0.25f).SetEase(Ease.InOutSine);
                        }

                        if (verticalInput > 0)
                        {
                            StartTrails();
                        }
                        else
                        {
                            StopTrails();
                        }
                        
                        
                        var4 = false;
                        var5 = false;
                        var6 = true;
                    }
                }
                
            }
            else
            { 
                var4 = false;
                var5 = false;
                var6 = false;
                
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
                            else
                            {
                                //Do smoke effects
                                Debug.LogWarning("Üst rampaya çıkamadın :("); 
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
