using System;
using TimelessEchoes.Buffs;
using TimelessEchoes.Hero;
using TimelessEchoes.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TimelessEchoes.Enemies
{
    /// <summary>
    ///     Handles spawning and timed kills for reaper animations.
    /// </summary>
    public class ReaperManager : MonoBehaviour
    {
        private GameObject target;
        private Action onKill;
        private bool fromHero;

        [SerializeField] private float moveDistance = 0.5f;
        [SerializeField] private float moveDownDistance;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveDownSpeed = 8f;

        private Vector3 startPosition;
        private float moved;
        private float movedDown;

        private void OnEnable()
        {
            startPosition = transform.position;
            moved = 0f;
            movedDown = 0f;
        }

        private void Update()
        {
            var step = moveSpeed * Time.deltaTime;
            var downStep = moveDownSpeed * Time.deltaTime;

            if (moved < moveDistance)
            {
                var remaining = moveDistance - moved;
                var horizStep = step;
                if (horizStep > remaining) horizStep = remaining;
                var direction = Mathf.Sign(transform.localScale.x);
                transform.position += transform.right * horizStep * direction;
                moved += horizStep;
            }

            if (moveDownDistance > 0f && movedDown < moveDownDistance)
            {
                var remaining = moveDownDistance - movedDown;
                var vertStep = downStep;
                if (vertStep > remaining) vertStep = remaining;
                transform.position += Vector3.down * vertStep;
                movedDown += vertStep;
            }
        }

        /// <summary>
        ///     Initialize the reaper for a target.
        /// </summary>
        /// <param name="target">Target object to reap.</param>
        /// <param name="fromHero">True if spawned from the hero.</param>
        /// <param name="onKill">Callback invoked after the target is killed.</param>
        public void Init(GameObject target, bool fromHero = false, Action onKill = null)
        {
            this.target = target;
            this.onKill = onKill;
            this.fromHero = fromHero;
        }

        /// <summary>
        ///     Animator event used to kill the assigned target.
        /// </summary>
        public void KillTarget()
        {
            if (target == null) return;
            var hp = target.GetComponent<IHasHealth>();
            var dmg = target.GetComponent<IDamageable>();
            var tracker = GameplayStatTracker.Instance ??
                          FindFirstObjectByType<GameplayStatTracker>();

            if (hp != null && dmg != null && hp.CurrentHealth > 0f)
            {
                var amount = hp.CurrentHealth;
                var enemy = target.GetComponent<Enemy>();
                if (enemy != null && enemy.Stats != null)
                    amount += enemy.Stats.defense + 1f;
                dmg.TakeDamage(amount);
                if (fromHero)
                {
                    tracker?.AddDamageDealt(amount);
                    var buff = BuffManager.Instance ??
                               FindFirstObjectByType<BuffManager>();
                    var hero = HeroController.Instance ??
                               FindFirstObjectByType<HeroController>();
                    var heroHp = hero != null ? hero.GetComponent<HeroHealth>() : null;
                    if (buff != null && heroHp != null)
                    {
                        var ls = buff.LifestealPercent;
                        if (ls > 0f)
                            heroHp.Heal(amount * ls / 100f);
                    }
                }
            }

            tracker?.AddTimesReaped();

            onKill?.Invoke();
            target = null;
        }

        /// <summary>
        ///     Animator event to destroy the reaper instance.
        /// </summary>
        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        /// <summary>
        ///     Spawns a new reaper targeting the given object.
        /// </summary>
        public static ReaperManager Spawn(GameObject prefab, GameObject target, Transform parent = null,
            bool fromHero = false, Action onKill = null, Vector3? positionOffset = null)
        {
            if (prefab == null || target == null) return null;
            var offset = positionOffset ?? Vector3.zero;

            // Randomly flip the horizontal spawn position so the reaper may
            // appear from either side of the target.
            var fromRight = Random.value < 0.5f;
            if (fromRight) offset.x = -offset.x;

            var obj = Instantiate(prefab, target.transform.position + offset, Quaternion.identity, parent);
            if (fromRight)
            {
                // Flip horizontally by inverting the x scale rather than rotating,
                // as this is a 2D sprite.
                var localScale = obj.transform.localScale;
                localScale.x *= -1f;
                obj.transform.localScale = localScale;
            }

            var mgr = obj.GetComponent<ReaperManager>();
            if (mgr != null)
                mgr.Init(target, fromHero, onKill);
            return mgr;
        }
    }
}