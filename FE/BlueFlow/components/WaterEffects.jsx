import { motion } from "framer-motion";
import { useEffect, useState, useMemo, useRef } from "react";

function WaterEffects() {
  const [ripples, setRipples] = useState([]);
  const [isVisible, setIsVisible] = useState(true);
  const containerRef = useRef(null);

  // Intersection Observer to pause animations when not visible
  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry]) => {
        setIsVisible(entry.isIntersecting);
      },
      { threshold: 0.1 }
    );

    if (containerRef.current) {
      observer.observe(containerRef.current);
    }

    return () => {
      if (containerRef.current) {
        observer.unobserve(containerRef.current);
      }
    };
  }, []);

  // Generate bubbles - Optimized count for performance
  const largeBubbles = useMemo(() => Array.from({ length: 12 }, (_, i) => ({
    id: i,
    left: Math.random() * 100,
    size: Math.random() * 60 + 25,
    delay: Math.random() * 10,
    duration: Math.random() * 15 + 20,
    xOffset: Math.random() * 100 - 50,
  })), []);

  const smallBubbles = useMemo(() => Array.from({ length: 35 }, (_, i) => ({
    id: i,
    left: Math.random() * 100,
    size: Math.random() * 20 + 5,
    delay: Math.random() * 12,
    duration: Math.random() * 12 + 18,
    xOffset: Math.random() * 60 - 30,
  })), []);

  // Auto-generate ripples - Optimized frequency
  useEffect(() => {
    if (!isVisible) return;
    
    const interval = setInterval(() => {
      setRipples((prev) => [
        ...prev.slice(-3), // Keep last 3 ripples
        {
          id: Date.now(),
          left: Math.random() * 100,
          top: Math.random() * 50 + 30,
        },
      ]);
    }, 2500);

    return () => clearInterval(interval);
  }, [isVisible]);

  // Water particles - Optimized count
  const waterParticles = useMemo(() => Array.from({ length: 25 }, (_, i) => ({
    id: i,
    left: Math.random() * 100,
    top: Math.random() * 100,
    delay: Math.random() * 5,
    duration: Math.random() * 4 + 3,
    yOffset1: Math.random() * 100 - 50,
    yOffset2: Math.random() * 200 - 100,
    xOffset1: Math.random() * 50 - 25,
    xOffset2: Math.random() * 100 - 50,
  })), []);

  // Floating Water Orbs - Optimized count
  const waterOrbs = useMemo(() => Array.from({ length: 8 }, (_, i) => ({
    id: i,
    left: Math.random() * 100,
    top: Math.random() * 100,
    width: Math.random() * 400 + 300,
    height: Math.random() * 400 + 300,
    xOffset: Math.random() * 300 - 150,
    yOffset: Math.random() * 300 - 150,
    duration: 20 + i * 3,
    delay: i * 0.8,
  })), []);

  return (
    <div ref={containerRef} className="fixed inset-0 pointer-events-none z-[1]">
      {/* Background Water Gradient - More prominent */}
      <div className="absolute inset-0" style={{ willChange: 'opacity', contain: 'layout style paint' }}>
        <motion.div
          className="absolute inset-0"
          style={{
            background: 'radial-gradient(ellipse at 50% 50%, rgba(59, 130, 246, 0.4) 0%, transparent 65%)',
            willChange: 'transform, opacity',
            transform: 'translateZ(0)',
          }}
          animate={isVisible ? {
            scale: [1, 1.3, 1],
            opacity: [0.3, 0.45, 0.3],
          } : {}}
          transition={{
            duration: 10,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
      </div>

      {/* Floating Water Orbs - Optimized with useMemo */}
      <div className="absolute inset-0 overflow-hidden" style={{ willChange: 'transform', contain: 'layout style paint' }}>
        {waterOrbs.map((orb) => (
          <motion.div
            key={`orb-${orb.id}`}
            className="absolute rounded-full blur-3xl"
            style={{
              left: `${orb.left}%`,
              top: `${orb.top}%`,
              width: `${orb.width}px`,
              height: `${orb.height}px`,
              background: `radial-gradient(circle, rgba(${59 + orb.id * 8}, ${130 + orb.id * 3}, 246, 0.2), transparent)`,
              willChange: 'transform, opacity',
              transform: 'translateZ(0)',
            }}
            animate={isVisible ? {
              x: [0, orb.xOffset],
              y: [0, orb.yOffset],
              scale: [1, 1.4, 1],
              opacity: [0.12, 0.3, 0.12],
            } : {}}
            transition={{
              duration: orb.duration,
              repeat: Infinity,
              ease: "easeInOut",
              delay: orb.delay,
            }}
          />
        ))}
      </div>

      {/* Water Ripples - Optimized */}
      <div className="absolute inset-0 overflow-hidden" style={{ willChange: 'transform', contain: 'layout style paint' }}>
        {ripples.map((ripple) => (
          <motion.div
            key={ripple.id}
            className="absolute rounded-full border-2 border-sky-400/80 shadow-lg shadow-sky-400/30"
            style={{
              left: `${ripple.left}%`,
              top: `${ripple.top}%`,
              width: '0px',
              height: '0px',
              willChange: 'transform, opacity',
              transform: 'translateZ(0)',
            }}
            animate={isVisible ? {
              width: ['0px', '350px', '500px'],
              height: ['0px', '350px', '500px'],
              opacity: [1, 0.7, 0],
            } : {}}
            transition={{
              duration: 2,
              ease: "easeOut",
            }}
          />
        ))}
      </div>

      {/* Water Particles - Optimized with pre-calculated values */}
      <div className="absolute inset-0 overflow-hidden" style={{ willChange: 'transform', contain: 'layout style paint' }}>
        {waterParticles.map((particle) => (
          <motion.div
            key={particle.id}
            className="absolute w-3 h-3 rounded-full bg-sky-300/70 shadow-md shadow-sky-300/40"
            style={{
              left: `${particle.left}%`,
              top: `${particle.top}%`,
              willChange: 'transform, opacity',
              transform: 'translateZ(0)',
            }}
            animate={isVisible ? {
              y: [
                particle.top,
                particle.top + particle.yOffset1,
                particle.top + particle.yOffset2,
              ],
              x: [
                particle.left,
                particle.left + particle.xOffset1,
                particle.left + particle.xOffset2,
              ],
              scale: [0, 1.2, 0],
              opacity: [0, 0.8, 0],
            } : {}}
            transition={{
              duration: particle.duration,
              delay: particle.delay,
              repeat: Infinity,
              ease: "easeOut",
            }}
          />
        ))}
      </div>

      {/* Floating Bubbles - Large - Optimized with pre-calculated xOffset */}
      <div className="absolute inset-0 overflow-hidden" style={{ willChange: 'transform', contain: 'layout style paint' }}>
        {largeBubbles.map((bubble) => (
          <motion.div
            key={bubble.id}
            className="absolute rounded-full bg-gradient-to-br from-sky-300/65 to-blue-400/55 backdrop-blur-sm border-2 border-white/35 shadow-xl shadow-sky-300/30"
            style={{
              left: `${bubble.left}%`,
              bottom: '-60px',
              width: `${bubble.size}px`,
              height: `${bubble.size}px`,
              willChange: 'transform, opacity',
              transform: 'translateZ(0)',
            }}
            animate={isVisible ? {
              y: [-60, -1200],
              x: [0, bubble.xOffset],
              scale: [1, 1.3, 0.9],
              opacity: [0, 0.85, 0.85, 0],
            } : {}}
            transition={{
              duration: bubble.duration,
              delay: bubble.delay,
              repeat: Infinity,
              ease: "linear",
            }}
          />
        ))}
      </div>

      {/* Small bubbles - Optimized with pre-calculated xOffset */}
      <div className="absolute inset-0 overflow-hidden" style={{ willChange: 'transform', contain: 'layout style paint' }}>
        {smallBubbles.map((bubble) => (
          <motion.div
            key={`small-${bubble.id}`}
            className="absolute rounded-full bg-sky-200/55 border border-white/25 shadow-lg shadow-sky-200/20"
            style={{
              left: `${bubble.left}%`,
              bottom: '-20px',
              width: `${bubble.size}px`,
              height: `${bubble.size}px`,
              willChange: 'transform, opacity',
              transform: 'translateZ(0)',
            }}
            animate={isVisible ? {
              y: [-20, -1000],
              x: [0, bubble.xOffset],
              opacity: [0, 0.75, 0.75, 0],
            } : {}}
            transition={{
              duration: bubble.duration,
              delay: bubble.delay,
              repeat: Infinity,
              ease: "linear",
            }}
          />
        ))}
      </div>
    </div>
  );
}

export default WaterEffects;

